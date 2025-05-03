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

    public async Task<IActionResult> OnGetDashboardViewAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var dashboardData = await _analyticsService.GetDashboardAnalyticsAsync(startDate, endDate);
            return Partial("Shared/Analytics/Dashboard/_Dashboard", dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке представления дашборда");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetTopProductsViewAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var topProducts = await _analyticsService.GetTopProductsAsync(
                10,
                startDate ?? DateTime.UtcNow.AddDays(-30),
                endDate ?? DateTime.UtcNow);
            
            return Partial("Shared/Analytics/Dashboard/_TopProductsChart", topProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке представления топ товаров");
            return StatusCode(500);
        }
    }
    
    public async Task<IActionResult> OnGetSalesViewAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            //Not implemented yet
            return Partial("Shared/Analytics/Sales/_Sales", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке представления продаж");
            return StatusCode(500);
        }
    }
    
    public async Task<IActionResult> OnGetReportsViewAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            //Not implemented yet
            return Partial("Shared/Analytics/Reports/_Reports", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке представления отчетов");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetOrdersViewAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            //Not implemented yet
            return Partial("Shared/Analytics/Orders/_Orders", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке представления заказов");
            return StatusCode(500);
        }
    }
    
    public async Task<IActionResult> OnGetTopProductsDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var topProducts = await _analyticsService.GetTopProductsAsync(
                10,
                startDate ?? DateTime.UtcNow.AddDays(-30),
                endDate ?? DateTime.UtcNow);
            return new JsonResult(topProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных топ товаров");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetSalesDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            //Not implemented yet
            return new JsonResult(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных продаж");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetOrdersDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            //Not implemented yet
            return new JsonResult(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных заказов");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetReportsDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            //Not implemented yet
            return new JsonResult(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных отчетов");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetTrendDataAsync(DateTime? startDate, DateTime? endDate, string interval = "1d")
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