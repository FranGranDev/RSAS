using Application.Areas.Identity.Data;
using Application.Model.Orders;
using Application.Services;
using Application.ViewModel.Catalog;
using Application.ViewModel.Orders;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Pages
{
    public class OrderInfoModel : PageModel
    {
        public OrderInfoModel(DataManager dataManager, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.dataManager = dataManager;
            this.configuration = configuration;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly DataManager dataManager;
        private readonly IConfiguration configuration;


        [BindProperty(SupportsGet = true)]
        public IEnumerable<CatalogItemViewModel> Products { get; set; }
        [BindProperty(SupportsGet = true)]
        public OrderViewModel Order { get; set; }
        [BindProperty(SupportsGet = true)]
        public DeliveryViewModel Delivery { get; set; }
        [BindNever]
        public CompanyViewModel AppCompany { get; set; }

        public async Task<IActionResult> OnGet(int orderId)
        {
            Order order = dataManager.Orders.Get(orderId);
            Order = new(order);

            var user = await userManager.GetUserAsync(HttpContext.User);
            if (order.UserId != user.Id)
            {
                return Redirect("/Identity/Account/AccessDenied");
            }

            Products = order.Products
                .Select(x => new CatalogItemViewModel(order.Type)
                {
                    Name = x.ProductName,
                    Description = x.ProductDescription,
                    TakenCount = x.Quantity,
                    WholesalePrice = x.ProductPrice,
                    RetailPrice = x.ProductPrice,
                    TotalPrice = x.ProductPrice * x.Quantity,
                });
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

            AppCompany = new CompanyViewModel(configuration)
            {
                Disabled = true,
            };

            ModelState.Clear();

            return Page();
        }
    }
}
