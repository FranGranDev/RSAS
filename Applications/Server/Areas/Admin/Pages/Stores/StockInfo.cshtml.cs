using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Application.Areas.Admin.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    [BindProperties(SupportsGet = true)]
    public class StockInfoModel : PageModel
    {
        public StockInfoModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }
        private readonly DataManager dataManager;


        public string SearchString { get; set; }
        public string SortOrder { get; set; }

        public QuantityStockViewModel Stock { get; set; }
        public IEnumerable<QuantityProductViewModel> Products { get; set; }


        public void OnGet(int id)
        {
            Stock stock = dataManager.Stocks.Get(id);
            Stock = new QuantityStockViewModel(stock);
            Products = stock.StockProducts
                .Select(x => new QuantityProductViewModel
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    Description = x.Product.Description,
                    RetailPrice = x.Product.RetailPrice,
                    WholesalePrice = x.Product.WholesalePrice,
                    Quantity = x.Quantity,
                });



            SearchString = HttpContext.Session.GetString("search") ?? "";
            if (!string.IsNullOrEmpty(SearchString))
            {
                Products = Products.Where(p => p.Name.Contains(SearchString));
            }

            SortOrder = HttpContext.Session.GetString("sort") ?? "";
            switch (SortOrder)
            {
                case "Name_desc":
                    Products = Products.OrderByDescending(p => p.Name);
                    break;
                case "Description":
                    Products = Products.OrderBy(p => p.Description);
                    break;
                case "Description_desc":
                    Products = Products.OrderByDescending(p => p.Description);
                    break;
                case "WholesalePrice":
                    Products = Products.OrderBy(p => p.WholesalePrice);
                    break;
                case "WholesalePrice_desc":
                    Products = Products.OrderByDescending(p => p.WholesalePrice);
                    break;
                case "RetailPrice":
                    Products = Products.OrderBy(p => p.RetailPrice);
                    break;
                case "RetailPrice_desc":
                    Products = Products.OrderByDescending(p => p.RetailPrice);
                    break;
                case "Quantity":
                    Products = Products.OrderBy(p => p.Quantity);
                    break;
                case "Quantity_desc":
                    Products = Products.OrderByDescending(p => p.Quantity);
                    break;
                default:
                    Products = Products.OrderBy(p => p.Name);
                    break;
            }
        }

        public IActionResult OnPostSort(string sortOrder)
        {
            HttpContext.Session.SetString("sort", sortOrder);

            return RedirectToPage(new { id = Stock.Id });
        }
        public IActionResult OnPostSearch()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                HttpContext.Session.SetString("search", SearchString);
            }
            else
            {
                HttpContext.Session.Remove("search");
            }

            return RedirectToPage(new { id = Stock.Id });
        }
        public string GetSortOrder(string column)
        {
            if (SortOrder == $"{column}_desc")
            {
                return column;
            }
            else
            {
                return $"{column}_desc";
            }
        }
    }
}
