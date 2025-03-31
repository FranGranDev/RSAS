using Application.Areas.Identity.Data;
using Application.Model.Orders;
using Application.Model.Sales;
using Application.Services;
using Application.ViewModel.Sales;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Application.Areas.Admin.Pages.Analytics
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        public IndexModel(DataManager dataManager, UserManager<AppUser> userManager, ICompanyStore companyStore, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.dataManager = dataManager;
            this.userManager = userManager;
            this.companyStore = companyStore;
        }

        private readonly DataManager dataManager;
        private readonly ICompanyStore companyStore;
        private readonly UserManager<AppUser> userManager;
        private readonly IMemoryCache memoryCache;

        public List<SelectListItem> SalesTypesList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() {Value = "-1", Text = "Все продажи", Selected = true, },
                    new SelectListItem() {Value = "0", Text = "Только Розничные"},
                    new SelectListItem() {Value = "1", Text = "Только Оптовые" },
                };
            }
        }
        public List<SelectListItem> AnalyticsTypesList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() {Value = "0", Text = "По продажам", Selected = true, },
                    new SelectListItem() {Value = "1", Text = "По товарам"},
                };
            }
        }


        public string SortBy
        {
            get
            {
                return HttpContext.Session.GetString("sales_sortBy") ?? "";
            }
            set
            {
                HttpContext.Session.SetString("sales_sortBy", value);
            }
        }
        public string SortOrder
        {
            get
            {
                return HttpContext.Session.GetString("sales_sortOrder") ?? "";
            }
            set
            {
                HttpContext.Session.SetString("sales_sortOrder", value);
            }
        }
        public int SaleTypeFilter
        {
            get
            {
                return HttpContext.Session.GetInt32("sales_filter") ?? -1;
            }
            set
            {
                HttpContext.Session.SetInt32("sales_filter", (int)value);
            }
        }
        public int AnalyticsTypeFilter
        {
            get
            {
                return HttpContext.Session.GetInt32("analytics_filter") ?? 0;
            }
            set
            {
                HttpContext.Session.SetInt32("analytics_filter", (int)value);
            }
        }
        public string StartDate
        {
            get
            {
                return HttpContext.Session.GetString("sales_start_date") ?? string.Empty;
            }
            set
            {
                HttpContext.Session.SetString("sales_start_date", value);
            }
        }
        public string EndDate
        {
            get
            {
                return HttpContext.Session.GetString("sales_end_date") ?? string.Empty;
            }
            set
            {
                HttpContext.Session.SetString("sales_end_date", value);
            }
        }


        public IEnumerable<SaleViewModel> Sales
        {
            get
            {
                IEnumerable<SaleViewModel> sales;
                if(memoryCache.TryGetValue("sales", out sales))
                {
                    return sales;
                }

                return new List<SaleViewModel>();
            }
        }
        public async Task LoadSales()
        {
            var sales = await dataManager.Sales.All
                .Select(x => new SaleViewModel(x, null))
                .ToListAsync();

            memoryCache.Set("sales", sales);
        }

        public async Task<IActionResult> OnGet()
        {
            await LoadSales();

            return Page();
        }

        public IActionResult OnGetSales()
        {
            var sales = Sales;

            sales = GetFilteredByDate(sales);
            sales = GetFilteredByType(sales);

            switch(AnalyticsTypeFilter)
            {
                default:
                    {
                        sales = GetSorted(sales);
                        return Partial("_SalesPartial", sales);
                    }
                case 1:
                    {
                        var products = GetSaleProducts(sales);
                        products = GetSortedProducts(products);

                        return Partial("_ProductSalesPartial", products);
                    }
            }
        }
        public IActionResult OnGetAnalytics()
        {
            var sales = Sales;

            sales = GetFilteredByDate(sales);
            sales = GetFilteredByType(sales);
            sales = GetSorted(sales);


            var products = sales.SelectMany(x => x.Products);
            string period = "За все время";
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                period = $"с {StartDate} по {EndDate}";
            }

            if (sales.Count() > 0)
            {
                var analytics = new SalesAnalyticsViewModel()
                {
                    Period = period,
                    Revenue = products.Sum(x => x.Quantity * x.ProductPrice),
                    ProductsCount = products.Sum(x => x.Quantity),
                    AvgRevenue = sales.Select(x => x.Products.Sum(x => x.Quantity * x.ProductPrice)).Average(),
                    SalesCount = sales.Count(),
                };

                return Partial("_SalesAnalyticsPartial", analytics);
            }
            else
            {
                return Partial("_EmptySalesAnalyticsPartial");
            }
        }
        public async Task<IActionResult> OnGetInfo(int id)
        {
            SaleViewModel sale = Sales.First(x => x.Id == id);

            if (sale.SaleType == SaleTypes.Wholesale)
            {
                var user = await userManager.FindByIdAsync(sale.UserId);
                Company company = companyStore.Get(user);

                if (company != null)
                {
                    sale.CompanyInfo = new CompanyViewModel(company)
                    {
                        Disabled = true,
                    };
                }
            }

            return Partial("_SaleInfoPartial", sale);
        }



        public IActionResult OnPostSort(string sortBy, string sortOrder)
        {
            SortBy = sortBy;
            SortOrder = sortOrder;

            return OnGetSales();
        }
        public IActionResult OnPostSaleType(int value)
        {
            SaleTypeFilter = value;

            return OnGetSales();
        }
        public IActionResult OnPostAnalyticsType(int value)
        {
            AnalyticsTypeFilter = value;

            return OnGetSales();
        }
        public IActionResult OnPostSetDate(string? startDate, string? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return OnGetSales();
            }

            StartDate = startDate;
            EndDate = endDate;

            return OnGetSales();
        }
        public IActionResult OnPostClearDate()
        {
            StartDate = string.Empty;
            EndDate = string.Empty;

            return OnGetSales();
        }


        private IEnumerable<SaleViewModel> GetSorted(IEnumerable<SaleViewModel> sales)
        {
            string sortBy = SortBy;
            string sortOrder = SortOrder;

            switch (sortBy)
            {
                case "type":
                    if (sortOrder == "asc")
                    {
                        sales = sales.OrderBy(p => p.SaleType);
                    }
                    else
                    {
                        sales = sales.OrderByDescending(p => p.SaleType);
                    }
                    break;
                case "date":
                    if (sortOrder == "asc")
                    {
                        sales = sales.OrderBy(p => p.SaleDate);
                    }
                    else
                    {
                        sales = sales.OrderByDescending(p => p.SaleDate);
                    }
                    break;
                case "price":
                    if (sortOrder == "asc")
                    {
                        sales = sales.OrderBy(p => p.Amount);
                    }
                    else
                    {
                        sales = sales.OrderByDescending(p => p.Amount);
                    }
                    break;
                case "quantity":
                    if (sortOrder == "asc")
                    {
                        sales = sales.OrderBy(p => p.Quantity);
                    }
                    else
                    {
                        sales = sales.OrderByDescending(p => p.Quantity);
                    }
                    break;
                default:
                    sales = sales.OrderByDescending(p => p.OrderDate);
                    break;
            }

            return sales;
        }
        private IEnumerable<SaleViewModel> GetFilteredByType(IEnumerable<SaleViewModel> sales)
        {
            int? filter = SaleTypeFilter;

            if (filter == null || filter == -1)
                return sales;

            return sales.Where(x => (int)x.SaleType == filter);
        }
        private IEnumerable<SaleViewModel> GetFilteredByDate(IEnumerable<SaleViewModel> sales)
        {
            string startDateStr = StartDate;
            string endDateStr = EndDate;

            if(string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return sales;
            }



            if (DateTime.TryParseExact(startDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) &&
                DateTime.TryParseExact(endDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                return sales.Where(x => x.SaleDate >= startDate && x.SaleDate <= endDate);
            }

            return sales;
        }


        private IEnumerable<SaleProductViewModel> GetSaleProducts(IEnumerable<SaleViewModel> sales)
        {
            var products = sales
                .SelectMany(sale => sale.Products)
                .GroupBy(product => product.ProductId)
                .Select(group => new SaleProductViewModel()
                {
                    Name = group.First().ProductName,
                    Description = group.First().ProductDescription,
                    Price = group.First().ProductPrice,
                    Quantity = group.Sum(x => x.Quantity),
                    Income = group.Sum(x => x.Quantity * x.ProductPrice),
                });

            return products;
        }
        private IEnumerable<SaleProductViewModel> GetSortedProducts(IEnumerable<SaleProductViewModel> products)
        {
            string sortBy = SortBy;
            string sortOrder = SortOrder;

            switch (sortBy)
            {
                case "name":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Name);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Name);
                    }
                    break;
                case "description":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Description);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Description);
                    }
                    break;
                case "price":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Price);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Price);
                    }
                    break;
                case "quantity":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Quantity);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Quantity);
                    }
                    break;
                case "income":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Income);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Income);
                    }
                    break;
                default:
                    products = products.OrderByDescending(p => p.Income);
                    break;
            }

            return products;
        }


        public enum AnalyticsTypes
        {
            Sales,
            Products,
        }
    }
}
