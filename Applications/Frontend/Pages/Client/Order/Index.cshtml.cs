using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Frontend.Pages.Client.Order
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IEnumerable<OrderDto> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Orders = await _apiService.GetAsync<IEnumerable<OrderDto>>("api/orders/my");
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
        }
    }
} 