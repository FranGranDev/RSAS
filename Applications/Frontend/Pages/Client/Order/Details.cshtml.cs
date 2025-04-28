using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Frontend.Pages.Client.Order
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IApiService _apiService;

        public DetailsModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public OrderDto Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                Order = await _apiService.GetAsync<OrderDto>($"api/orders/{id}");
                
                // Проверяем, принадлежит ли заказ текущему пользователю
                if (Order.UserId != userId)
                {
                    return Forbid();
                }

                return Page();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
} 