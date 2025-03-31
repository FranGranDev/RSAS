using Application.Areas.Identity.Data;
using Application.Model.Orders;
using Application.Services;
using Application.ViewModel.Catalog;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace Application.Pages.Companies
{
    public class OrderingModel : CatalogProducts
    {
        public OrderingModel(DataManager dataManager, ICompanyStore companyStore, UserManager<AppUser> userManager, IMemoryCache memoryCache, IConfiguration configuration) : base(memoryCache)
        {
            this.dataManager = dataManager;
            this.companyStore = companyStore;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        private readonly ICompanyStore companyStore;
        private readonly DataManager dataManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;


        [BindProperty]
        public IEnumerable<CatalogItemViewModel> Products { get; set; }
        [BindProperty]
        public DeliveryViewModel Delivery { get; set; }
        [BindProperty]
        public ContactInfoViewModel Contact { get; set; }
        [BindNever]
        public CompanyViewModel Company { get; set; }
        [BindNever]
        public CompanyViewModel AppCompany { get; set; }


        public async Task<IActionResult> OnGet()
        {
            Products = CachedProducts;
            if (Products == null)
                return Page();
            Products = Products
                .Where(x => x.TakenCount > 0);
            Products
                .ToList()
                .ForEach(x => x.TotalPrice = x.TakenCount * x.WholesalePrice);

            var user = await userManager.GetUserAsync(HttpContext.User);
            Company company = companyStore.Get(user);

            Contact = new ContactInfoViewModel()
            {
                Phone = company.Phone,
            };

            Company = new CompanyViewModel(company)
            {
                Disabled = true,
            };

            AppCompany = new CompanyViewModel(configuration)
            {
                Disabled = true,
            };

            DateTime date = DateTime.Now.AddDays(1);
            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);

            Delivery = new DeliveryViewModel()
            {                
                DeliveryDate = date,
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

            ModelState.Clear();

            if (Products == null || Products.Count() == 0)
            {
                return Page();
            }
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(HttpContext.User);

            Order order = new Order()
            {
                UserId = user.Id,
                ClientName = Contact.FullName,
                ContactPhone = Contact.Phone,
                Type = Model.Sales.SaleTypes.Wholesale,
                PaymentType = Order.PaymentTypes.Bank,
            };
            Delivery delivery = new Delivery()
            {
                DeliveryDate = Delivery.DeliveryDate,
                City = Delivery.City,
                Street = Delivery.Street,
                House = Delivery.House,
                Flat = Delivery.Flat,
                PostalCode = Delivery.PostalCode,                
            };

            order = await dataManager.Orders.CreateOrder(order, Products, delivery);
            dataManager.Orders.Save(order);

            CachedProducts = null;

            return RedirectToPage("/OrderDone");
        }
    }
}
