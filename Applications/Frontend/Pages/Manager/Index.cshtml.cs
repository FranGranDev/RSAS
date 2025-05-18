using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Services.Api;
using Application.DTOs;
using Frontend.Models.Analytics;

namespace Frontend.Pages.Manager;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IApiService _apiService;

    public IndexModel(IAnalyticsService analyticsService, IApiService apiService)
    {
        _analyticsService = analyticsService;
        _apiService = apiService;
    }

    public DashboardViewModel Dashboard { get; set; }
    public IEnumerable<OrderDto> RecentOrders { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var startDate = DateTime.Today; // начало дня (00:00:00)
            var endDate = DateTime.Today.AddDays(1).AddTicks(-1); // конец дня (23:59:59.9999999)
            
            Dashboard = await _analyticsService.GetDashboardAnalyticsAsync(startDate, endDate);
            
            // Получаем последние 3 заказа
            var orders = await _apiService.GetAsync<IEnumerable<OrderDto>>("api/orders");
            RecentOrders = orders
                .Where(x => x.State is Application.Models.Order.States.New or Application.Models.Order.States.OnHold or Application.Models.Order.States.InProcess)
                .OrderByDescending(o => o.OrderDate)
                .Take(3);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при загрузке данных";
            return Page();
        }
    }
} 