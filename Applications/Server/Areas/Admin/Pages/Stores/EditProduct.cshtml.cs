using Application.Services;
using Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Application.ViewModel.Data;
using Application.Model.Stocks;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Application.Areas.Admin.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    public class EditProductModel : PageModel
    {
        public EditProductModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        private readonly DataManager dataManager;


        [BindProperty]
        public QuantityProductViewModel Product { get; set; }
        [BindProperty]
        public StockViewModel Stock { get; set; }


        public void OnGet(int id, int stockId)
        {
            Stock stock = dataManager.Stocks.Get(stockId);
            Product product = dataManager.Products.Get(id);

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

            Product product = new Product
            {
                Id = Product.Id,
                Name = Product.Name,
                Description = Product.Description,
                WholesalePrice = Product.WholesalePrice,
                RetailPrice = Product.RetailPrice,
            };
            dataManager.Products.Save(product);

            dataManager.StockProducts.Save(new StockProducts
            {
                StockId = Stock.Id,
                ProductId = Product.Id,
                Quantity = Product.Quantity,
            });



            return RedirectToPage("StockInfo", new { id = Stock.Id });
        }
    }
}
