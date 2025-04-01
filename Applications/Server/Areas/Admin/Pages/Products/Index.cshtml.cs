using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DataManager dataManager;

        public IndexModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }


        public IEnumerable<QuantityProductViewModel> Products { get; set; }


        [BindProperty(SupportsGet = true)] public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)] public string SortOrder { get; set; }


        public void OnGet(string sortOrder)
        {
            Products = dataManager.Products.All
                .Select(x => new QuantityProductViewModel(x));


            SearchString = HttpContext.Session.GetString("search") ?? "";
            if (!string.IsNullOrEmpty(SearchString))
            {
                Products = Products.Where(p => p.Name.Contains(SearchString));
            }


            if (!string.IsNullOrEmpty(sortOrder))
            {
                SortOrder = sortOrder;
            }

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

            return RedirectToPage(new { sortOrder = SortOrder });
        }

        public IActionResult OnPostEdit(int id)
        {
            return RedirectToPage("Edit", new { id, returnUrl = Request.Path.ToString() });
        }

        public IActionResult OnPostDelete(int id)
        {
            var product = dataManager.Products.Get(id);

            if (product.StockProducts.Select(x => x.Quantity).Sum() == 0)
            {
                dataManager.Products.Delete(product);
                TempData["success"] = "Товар успешно удален";
            }
            else
            {
                TempData["error"] = "Невозможно удалить товар. Товар есть на складе";
            }

            return RedirectToPage(new { sortOrder = SortOrder });
        }

        public string GetSortOrder(string column)
        {
            if (SortOrder == $"{column}_desc")
            {
                return column;
            }

            return $"{column}_desc";
        }
    }
}