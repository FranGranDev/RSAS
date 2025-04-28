using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;

namespace Frontend.Pages.Manager.Order;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly IApiService _apiService;

    public IndexModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IEnumerable<OrderDto> Orders { get; set; } = new List<OrderDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            Orders = await _apiService.GetAsync<IEnumerable<OrderDto>>("api/orders");
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при загрузке заказов";
            return Page();
        }
    }
} 