using Application.Model;
using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Application.Areas.Admin.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    public class AddProductModel : PageModel
    {
        public AddProductModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }
        private readonly DataManager dataManager;


        [BindProperty(SupportsGet = true)]
        public StockViewModel Stock { get; set; }

        [BindProperty(SupportsGet = true)]
        public QuantityProductViewModel Product { get; set; }


        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }


        public IEnumerable<QuantityProductViewModel> Products { get; set; }


        public async Task<IActionResult> OnGet(int id, int productId)
        {
            Stock stock = dataManager.Stocks.Get(id);
            Stock = new StockViewModel(dataManager.Stocks.Get(id));
            Products = await dataManager.Products.All
                .Select(x => new QuantityProductViewModel(x)
                {
                    Quantity = x.StockProducts
                        .Where(x => x.StockId == stock.Id)
                        .Select(x => x.Quantity)
                        .Sum(),
                }).ToListAsync();

            if (productId == default)
            {
                Product = new QuantityProductViewModel
                {
                    Id = 0,
                    Name = "Товар не выбран",
                    Description = "-",
                    RetailPrice = 0,
                    WholesalePrice = 0,
                    Quantity = 0,
                };
            }
            else
            {
                Product = new QuantityProductViewModel(dataManager.Products.Get(productId))
                {
                    Quantity = stock.StockProducts.Where(x => x.ProductId == productId)
                        .Select(x => x.Quantity)
                        .Sum(),
                };
            }


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

            return Page();
        }
        public IActionResult OnPostSort(string sortOrder)
        {
            HttpContext.Session.SetString("sort", sortOrder);

            return RedirectToPage(new { id = Stock.Id, productId = Product.Id});
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

            return RedirectToPage(new { id = Stock.Id, productId = Product.Id });
        }
        public IActionResult OnPostSelect(int productId)
        {
            return RedirectToPage(new { id = Stock.Id, productId = productId });
        }
        public IActionResult OnPostAdd()
        {
            if(Product.Id == default)
            {
                TempData["error"] = "Товар не выбран";
                return RedirectToPage(new { id = Stock.Id, productId = 0 });
            }
            else if(Product.Quantity < 0)
            {
                TempData["error"] = "Количество товара не может быть отрицательным значением";
                return RedirectToPage(new { id = Stock.Id, productId = 0 });
            }

            dataManager.StockProducts.Save(new StockProducts
            {
                StockId = Stock.Id,
                ProductId = Product.Id,
                Quantity = Product.Quantity,
            });

            TempData["success"] = $"Количество выбранного товара изменено на {Product.Quantity}";

            return RedirectToPage(new { id = Stock.Id, productId = Product.Id });
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
