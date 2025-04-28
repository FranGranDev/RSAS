using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Frontend.Models.Catalog;
using Frontend.Services;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Client
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;
        private readonly ICartService _cartService;

        public IndexModel(IApiService apiService, ICartService cartService)
        {
            _apiService = apiService;
            _cartService = cartService;
        }

        public IEnumerable<OrderDto> LastOrders { get; set; }
        public CartViewModel Cart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Получаем последние 3 заказа
            LastOrders = await _apiService.GetAsync<IEnumerable<OrderDto>>("api/orders/my");
            LastOrders = LastOrders.OrderByDescending(o => o.OrderDate).Take(3);

            // Получаем корзину
            Cart = _cartService.GetOrCreateCart(userId);

            return Page();
        }
    }
} 