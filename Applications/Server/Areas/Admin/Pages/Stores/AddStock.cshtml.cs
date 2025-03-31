using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using System.Data;

namespace Application.Areas.Admin.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    public class AddStockModel : PageModel
    {
        public AddStockModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }


        private readonly DataManager dataManager;


        [BindProperty]
        public StockViewModel Stock { get; set; }

        public void OnGet()
        {
            Stock = new();
        }

        public async Task<IActionResult> OnPost()
        {
            if(dataManager.Stocks.All.Count(x => x.Name == Stock.Name) > 0)
            {
                ModelState.AddModelError("dublicate", "Stock with such name already exists.");
            }

            if(!ModelState.IsValid)
            {
                return Page();
            }

            Stock stock = new Stock
            {
                Name = Stock.Name,
                Location = Stock.Location,
                SaleType = Stock.SaleType,
            };

            dataManager.Stocks.Save(stock);

            TempData["success"] = "Склад успешно добавлен";

            return RedirectToPage("Index");
        }
    }
}
