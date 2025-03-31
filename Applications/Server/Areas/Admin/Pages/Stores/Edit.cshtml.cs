using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;

namespace Application.Areas.Admin.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        public EditModel(DataManager dataManager)
        {
            this.dataManager = dataManager; 
        }

        private readonly DataManager dataManager;


        [BindProperty]
        public StockViewModel Stock { get; set; }

        public async Task OnGetAsync(int id)
        {
            Stock currant = dataManager.Stocks.Get(id);

            Stock = new StockViewModel(currant);
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            if (!ModelState.IsValid)
                return RedirectToPage(new { id = Stock.Id });

            Stock updated = new Stock()
            {
                Id = Stock.Id,
                Name = Stock.Name,
                Location = Stock.Location,
                SaleType = Stock.SaleType,
            };

            dataManager.Stocks.Save(updated);

            TempData["success"] = "Склад успешно изменен";

            return RedirectToPage("StockInfo", new { id = Stock.Id });
        }
        public async Task<IActionResult> OnPostDelete()
        {
            Stock stock = dataManager.Stocks.Get(Stock.Id);

            int quantity = 0;
            if(stock.StockProducts != null)
            {
                quantity = (from productStock in stock.StockProducts
                 where productStock.StockId == stock.Id
                 select productStock.Quantity).Sum();
            }

            if(quantity > 0)
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
