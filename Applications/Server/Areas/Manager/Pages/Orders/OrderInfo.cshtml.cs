using System.Data;
using System.Linq;
using Application.Areas.Identity.Data;
using Application.Extensions;
using Application.Model.Orders;
using Application.Model.Sales;
using Application.Model.Stocks;
using Application.Models;
using Application.Services;
using Application.ViewModel.Catalog;
using Application.ViewModel.Data;
using Application.ViewModel.Orders;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Application.Model.Orders.Order;

namespace Application.Areas.Manager.Pages
{
    [Authorize(Roles = "Manager")]
    public class OrderInfoModel : PageModel
    {        
        public OrderInfoModel(DataManager dataManager, UserManager<AppUser> userManager, ICompanyStore companyStore)
        {
            this.userManager = userManager;
            this.dataManager = dataManager;
            this.companyStore = companyStore;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly ICompanyStore companyStore;
        private readonly DataManager dataManager;


 
        public IEnumerable<CatalogItemViewModel> Products { get; set; }

        [BindProperty(SupportsGet = true)]
        public OrderViewModel OrderView { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateViewModel DeliveryDate { get; set; }

        public DeliveryViewModel Delivery { get; set; }


        public List<SelectListItem> StatesList { get; } = new List<SelectListItem>()
        {
            new SelectListItem { Value = Order.States.New.ToString(), Text = Order.States.New.GetDisplayName() },
            new SelectListItem { Value = Order.States.InProcess.ToString(), Text = Order.States.InProcess.GetDisplayName() },
            new SelectListItem { Value = Order.States.OnHold.ToString(), Text = Order.States.OnHold.GetDisplayName() },
            new SelectListItem { Value = Order.States.Cancelled.ToString(), Text = Order.States.Cancelled.GetDisplayName() },
        };
        public List<SelectListItem> StockList { get; set; }


        public SaleTypes OrderType { get; set; }
        public ContactInfoViewModel ClientInfo { get; set; }
        public CompanyViewModel CompanyInfo { get; set; }


        [BindProperty(SupportsGet = true)]
        public int SelectedStock { get; set; }
        public int StockId
        {
            get
            {
                return HttpContext.Session.GetInt32("order_stock_id") ?? 0;
            }
            private set
            {
                HttpContext.Session.SetInt32("order_stock_id", value);
                SelectedStock = value;
            }
        }
        public int OrderId
        {
            get
            {
                return HttpContext.Session.GetInt32("order_id") ?? 0;
            }
            private set
            {
                HttpContext.Session.SetInt32("order_id", value);
            }
        }
        public bool IsEnough
        {
            get
            {
                return HttpContext.Session.GetInt32("is_enough") == 1;
            }
            private set
            {
                HttpContext.Session.SetInt32("is_enough", value ? 1 : 0);
            }
        }


        public async Task<IActionResult> OnGet(int orderId)
        {
            if (orderId != default)
            {
                OrderId = orderId;
            }
            

            Order order = dataManager.Orders.Get(OrderId);
            if(order == null)
            {
                return NotFound();
            }


            Stock stock = dataManager.Stocks.All.FirstOrDefault();
            if(stock == null)
            {
                return NotFound();
            }
            if (StockId != default)
            {
                stock = dataManager.Stocks.Get(StockId);
            }
            else
            {
                StockId = stock.Id;
            }

            OrderView = new(order);

            StockList = await dataManager.Stocks.All
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Name} | {x.Location} | {x.SaleType.GetDisplayName()}",
                })
                .ToListAsync();



            Products = order.Products
                .Select(x => new CatalogItemViewModel(order.Type)
                {
                    Name = x.ProductName,
                    Description = x.ProductDescription,
                    TakenCount = x.Quantity,
                    Quantity = stock.StockProducts
                        .Where(p => p.ProductId == x.ProductId)
                        .Select(x => x.Quantity)
                        .Sum(),
                    WholesalePrice = x.ProductPrice,
                    RetailPrice = x.ProductPrice,
                    TotalPrice = x.ProductPrice * x.Quantity,
                })
                .OrderBy(x => x.TotalPrice);

            OrderType = order.Type;

            ClientInfo = new ContactInfoViewModel()
            {
                FullName = order.ClientName,
                Phone = order.ContactPhone,
                Disabled = true,
            };

            if (order.Type == SaleTypes.Wholesale)
            {
                var user = await userManager.FindByIdAsync(order.UserId);
                Company company = companyStore.Get(user);

                if (company != null)
                {
                    CompanyInfo = new CompanyViewModel(company);
                }
            }

            Delivery delivery = order.Delivery;
            Delivery = new DeliveryViewModel()
            {
                DeliveryDate = delivery.DeliveryDate,
                House = delivery.House,
                City = delivery.City,
                Flat = delivery.Flat,
                Street = delivery.Street,
                PostalCode = delivery.PostalCode,
                Disabled = true,
            };
            DeliveryDate = new DateViewModel()
            {
                Date = delivery.DeliveryDate,
            };

            return Page();
        }

        public IActionResult OnPostDelivery()
        {
            Order order = dataManager.Orders.Get(OrderId);

            ModelState.Clear();
            if(!TryValidateModel(DeliveryDate))
            {
                TempData["error"] = "Указано невозможное время доставки";
                return RedirectToPage(new { orderId = OrderId });
            }

            order.Delivery.DeliveryDate = DeliveryDate.Date;
            order.ChangeDate = DateTime.Now;

            dataManager.Orders.Save(order);

            TempData["success"] = "Время доставки успешно изменено";

            return RedirectToPage(new { orderId = OrderId });
        }
        public IActionResult OnPostStock(int selectedStock)
        {
            StockId = selectedStock;

            return OnGetProducts();
        }

        public IActionResult OnPostHold()
        {
            Order order = dataManager.Orders.Get(OrderId);

            if(order.State == Order.States.InProcess)
            {
                return RedirectToPage(new { orderId = OrderId });
            }

            order.State = Order.States.OnHold;
            dataManager.Orders.Save(order);

            TempData["success"] = "Заявка отложена";

            return RedirectToPage(new { orderId = OrderId });
        }
        public IActionResult OnPostCancel()
        {
            Order order = dataManager.Orders.Get(OrderId);

            if (order.State == Order.States.InProcess)
            {
                return RedirectToPage(new { orderId = OrderId });
            }

            order.State = Order.States.Cancelled;
            dataManager.Orders.Save(order);

            TempData["success"] = "Заявка отменена";

            return RedirectToPage(new { orderId = OrderId });
        }
        public IActionResult OnPostReady()
        {
            Order order = dataManager.Orders.Get(OrderId);
            Stock stock = dataManager.Stocks.Get(StockId);

            switch(order.State)
            {
                case Order.States.Cancelled:
                    return RedirectToPage(new { orderId = OrderId });
                case Order.States.Completed:
                    return RedirectToPage(new { orderId = OrderId });
                case Order.States.InProcess:
                    return RedirectToPage(new { orderId = OrderId });
            }

            dataManager.Orders.ExecuteOrder(order, stock);

            TempData["success"] = "Заказ принят в работу";

            return RedirectToPage(new { orderId = OrderId });
        }
        public IActionResult OnPostComplete()
        {
            Order order = dataManager.Orders.Get(OrderId);
            Stock stock = dataManager.Stocks.Get(StockId);

            if (order.State != Order.States.InProcess)
            {
                return RedirectToPage(new { orderId = OrderId });
            }

            dataManager.Orders.CompleteOrder(order);
            dataManager.Sales.CreateSale(order);

            TempData["success"] = "Заказ успешно выполнен";

            return RedirectToPage(new { orderId = OrderId });
        }

        public IActionResult OnGetProducts()
        {
            Order order = dataManager.Orders.Get(OrderId);
            Stock stock = dataManager.Stocks.Get(StockId);

            string partialName;
            switch(order.State)
            {
                case Order.States.New:
                    partialName = "_StockOrderPartial";
                    break;
                case Order.States.OnHold:
                    partialName = "_StockOrderPartial";
                    break;
                default:
                    partialName = "_OrderPartial";
                    break;
            }


            if (stock == null || order == null)
            {
                return Partial(partialName, new List<CatalogItemViewModel>());
            }

            Products = order.Products
                .Select(x => new CatalogItemViewModel(order.Type)
                {
                    Name = x.ProductName,
                    Description = x.ProductDescription,
                    TakenCount = x.Quantity,
                    Quantity = stock.StockProducts
                        .Where(p => p.ProductId == x.ProductId)
                        .Select(x => x.Quantity)
                        .Sum(),
                    WholesalePrice = x.ProductPrice,
                    RetailPrice = x.ProductPrice,
                    TotalPrice = x.ProductPrice * x.Quantity,
                })
                .OrderBy(x => x.TotalPrice);


            IsEnough = true;
            foreach (var product in Products)
            {
                if (product.TakenCount > product.Quantity)
                {
                    IsEnough = false;
                    break;
                }
            }

            return Partial(partialName, Products);
        }
        public IActionResult OnGetOrderAction()
        {
            Order order = dataManager.Orders.Get(OrderId);

            OrderStateViewModel orderState = new OrderStateViewModel()
            {
                IsEnough = IsEnough,
                State = order.State,
            };

            return Partial("_OrderActionPartial", orderState);
        }
    }
}
