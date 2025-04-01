using Application.Models;
using Application.ViewModel.Catalog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Pages.User
{
    public class OrderingModel : CatalogProducts
    {
        private readonly IClientsStore clientsStore;
        private readonly DataManager dataManager;
        private readonly UserManager<AppUser> userManager;

        public OrderingModel(DataManager dataManager, IClientsStore clientsStore, UserManager<AppUser> userManager,
            IMemoryCache memoryCache) : base(memoryCache)
        {
            this.dataManager = dataManager;
            this.clientsStore = clientsStore;
            this.userManager = userManager;
        }


        [BindProperty] public IEnumerable<CatalogItemViewModel> Products { get; set; }

        [BindProperty] public DeliveryViewModel Delivery { get; set; }

        [BindProperty] public PaymentViewModel Payment { get; set; }

        [BindProperty] public ContactInfoViewModel Contact { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Products = CachedProducts;
            if (Products == null)
            {
                return Page();
            }

            Products = Products
                .Where(x => x.TakenCount > 0);
            Products
                .ToList()
                .ForEach(x => x.TotalPrice = x.TakenCount * x.RetailPrice);

            var user = await userManager.GetUserAsync(HttpContext.User);
            Client client = clientsStore.Get(user);

            Contact = new ContactInfoViewModel
            {
                FullName = client.FirstName + " " + client.LastName,
                Phone = client.Phone
            };
            Payment = new PaymentViewModel
            {
                Amount = Products.Select(x => x.TotalPrice).Sum(),
                PaymentType = Order.PaymentTypes.Cash
            };

            var date = DateTime.Now.AddDays(1);
            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);

            Delivery = new DeliveryViewModel
            {
                DeliveryDate = date
            };

            ModelState.Clear();

            return Page();
        }

        public async Task<IActionResult> OnPostOrder()
        {
            Products = CachedProducts
                .Where(x => x.TakenCount > 0);
            Products
                .ToList()
                .ForEach(x => x.TotalPrice = x.TakenCount * x.RetailPrice);

            if (Products == null || Products.Count() == 0)
            {
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(HttpContext.User);

            Order order = new Order
            {
                UserId = user.Id,
                ClientName = Contact.FullName,
                ContactPhone = Contact.Phone,
                Type = Model.Sales.SaleTypes.Retail,
                PaymentType = Payment.PaymentType
            };
            Delivery delivery = new Delivery
            {
                DeliveryDate = Delivery.DeliveryDate,
                City = Delivery.City,
                Street = Delivery.Street,
                House = Delivery.House,
                Flat = Delivery.Flat,
                PostalCode = Delivery.PostalCode
            };

            order = await dataManager.Orders.CreateOrder(order, Products, delivery);
            dataManager.Orders.Save(order);

            CachedProducts = null;

            return RedirectToPage("/OrderDone");
        }
    }
}