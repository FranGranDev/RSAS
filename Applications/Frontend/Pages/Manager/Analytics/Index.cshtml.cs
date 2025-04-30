using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Analytics;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IAnalyticsService analyticsService, ILogger<IndexModel> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Загрузка начальных данных для дашборда
            var dashboardData = await _analyticsService.GetDashboardAnalyticsAsync();
            ViewData["DashboardData"] = dashboardData;
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных аналитики");
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnGetDashboardAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var dashboardData = await _analyticsService.GetDashboardAnalyticsAsync(startDate, endDate);
            return Partial("Shared/Analytics/Dashboard/_Dashboard", dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных дашборда");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetSalesAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var salesData = await _analyticsService.GetSalesAnalyticsAsync(startDate, endDate);
            return Partial("Shared/Analytics/Sales/_Sales", salesData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных продаж");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetOrdersAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var ordersData = await _analyticsService.GetOrdersAnalyticsAsync(startDate, endDate);
            return Partial("Shared/Analytics/Orders/_Orders", ordersData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных заказов");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetReportsAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var reportsData = await _analyticsService.GetExtendedAnalyticsAsync(startDate, endDate);
            return Partial("Shared/Analytics/Reports/_Reports", reportsData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных отчетов");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetTrendAsync(DateTime? startDate, DateTime? endDate, string interval = "1d")
    {
        try
        {
            var trendData = await _analyticsService.GetSalesTrendAsync(
                startDate ?? DateTime.UtcNow.AddDays(-30),
                endDate ?? DateTime.UtcNow,
                interval);
            return new JsonResult(trendData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных тренда");
            return StatusCode(500);
        }
    }
} 