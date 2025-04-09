using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Services;

namespace Frontend.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;

        public IndexModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public List<OrderViewModel> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Orders = await _apiService.GetOrdersAsync();
                return Page();
            }
            catch (Exception ex)
            {
                // Здесь можно добавить логирование ошибок
                return RedirectToPage("/Error");
            }
        }
    }

    public class OrderViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
} 