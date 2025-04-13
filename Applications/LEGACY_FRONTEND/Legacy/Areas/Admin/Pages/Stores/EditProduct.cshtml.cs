using Application.Model.Stocks;
using Application.Models;
using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Admin.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    public class EditProductModel : PageModel
    {
        private readonly DataManager dataManager;

        public EditProductModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        [BindProperty] public QuantityProductViewModel Product { get; set; }

        [BindProperty] public StockViewModel Stock { get; set; }

        public void OnGet(int id, int stockId)
        {
            var stock = dataManager.Stocks.Get(stockId);
            var product = dataManager.Products.Get(id);

            Stock = new StockViewModel(stock);
            Product = new QuantityProductViewModel(product)
            {
                Quantity = stock.StockProducts
                    .Where(x => x.ProductId == product.Id)
                    .Select(x => x.Quantity)
                    .Sum()
            };
        }

        public IActionResult OnPostApply()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage(new { id = Product.Id, stockId = Stock.Id });
            }

            var product = new Product
            {
                Id = Product.Id,
                Name = Product.Name,
                Description = Product.Description,
                WholesalePrice = Product.WholesalePrice,
                RetailPrice = Product.RetailPrice,
                Barcode = Product.Barcode,
                Category = Product.Category
            };
            dataManager.Products.Save(product);

            dataManager.StockProducts.Save(new StockProducts
            {
                StockId = Stock.Id,
                ProductId = Product.Id,
                Quantity = Product.Quantity
            });

            return RedirectToPage("Edit", new { id = Stock.Id });
        }
    }
}