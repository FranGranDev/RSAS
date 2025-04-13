using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Admin.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly DataManager dataManager;

        public IndexModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }


        public IEnumerable<QuantityStockViewModel> Stocks { get; set; }

        public int StocksCount { get; private set; }
        public int ProductsCount { get; private set; }


        public void OnGet()
        {
            var query = dataManager.Stocks.All.Select(x => new QuantityStockViewModel(x));

            Stocks = query.ToList();

            StocksCount = Stocks.Count();
            ProductsCount = Stocks.Select(x => x.Quantity).Sum();
        }
    }
}