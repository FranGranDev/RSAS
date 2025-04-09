using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Services;

namespace Frontend.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ApiService _apiService;

        public LogoutModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _apiService.LogoutAsync();
            return RedirectToPage("/Index");
        }
    }
} 