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
    public class EditModel : PageModel
    {
        private readonly DataManager dataManager;

        public EditModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }


        [BindProperty] public StockViewModel Stock { get; set; }

        public async Task OnGetAsync(int id)
        {
            var currant = dataManager.Stocks.Get(id);

            Stock = new StockViewModel(currant);
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage(new { id = Stock.Id });
            }

            var updated = new Stock
            {
                Id = Stock.Id,
                Name = Stock.Name,
                Location = Stock.Location,
                SaleType = Stock.SaleType
            };

            dataManager.Stocks.Save(updated);

            TempData["success"] = "Склад успешно изменен";

            return RedirectToPage("StockInfo", new { id = Stock.Id });
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var stock = dataManager.Stocks.Get(Stock.Id);

            var quantity = 0;
            if (stock.StockProducts != null)
            {
                quantity = (from productStock in stock.StockProducts
                    where productStock.StockId == stock.Id
                    select productStock.Quantity).Sum();
            }

            if (quantity > 0)
            {
                TempData["error"] = "Невозможно удалить склад. На складе есть товары.";

                return RedirectToPage(new { id = Stock.Id });
            }

            dataManager.Stocks.Delete(stock.Id);

            TempData["success"] = "Склад успешно удален";

            return RedirectToPage("Index");
        }
    }
}