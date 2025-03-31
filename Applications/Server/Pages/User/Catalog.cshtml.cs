using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Catalog;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Application.Pages.User
{
    public class CatalogModel : CatalogBaseModel
    {
        public CatalogModel(DataManager dataManager, IMemoryCache memoryCache) : base(dataManager, memoryCache)
        {

        }


        public override IActionResult OnPostOrder()
        {
            var products = CachedProducts;
            if(products.Select(x => x.TakenCount).Sum() == 0)
            {
                ModelState.AddModelError("sum", "Добавьте как минимум один товар в корзину");
                return Page();
            }

            return RedirectToPage("Ordering");
        }


        public override List<CatalogItemViewModel> CreateProducts()
        {
            var products = dataManager.Stocks.All
                .Where(s => s.SaleType == Stock.Types.Retail)
                .SelectMany(s => s.StockProducts)
                .GroupBy(sp => sp.Product)
                .Select(g => new CatalogItemViewModel(Model.Sales.SaleTypes.Retail, g.Key)
                {
                    TakenCount = 0,
                    Quantity = g.Sum(sp => sp.Quantity)
                })
                .ToList();

            return new List<CatalogItemViewModel>(products);
        }
    }
}
