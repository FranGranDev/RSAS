using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Services;

namespace Frontend.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly ApiService _apiService;

        public DetailsModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public OrderViewModel Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Order = await _apiService.GetOrderAsync(id);
                if (Order == null)
                {
                    return NotFound();
                }
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }
    }
} 