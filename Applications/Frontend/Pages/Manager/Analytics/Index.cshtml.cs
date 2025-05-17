using Application.DTOs;
using Frontend.Models.Analytics;
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
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var dashboardData = await _analyticsService.GetDashboardAnalyticsAsync(formattedStartDate, formattedEndDate);
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
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var topProducts = await _analyticsService.GetTopProductsAsync(
                10,
                formattedStartDate ?? DateTime.UtcNow.AddDays(-30).Date,
                formattedEndDate ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1));
            
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
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var salesData = await _analyticsService.GetSalesAnalyticsAsync(formattedStartDate, formattedEndDate);
            var extendedAnalytics = await _analyticsService.GetExtendedSalesAnalyticsAsync(formattedStartDate, formattedEndDate);
            var abcAnalysis = await _analyticsService.GetProductAbcAnalysisAsync(formattedStartDate, formattedEndDate);
            var salesForecast = await _analyticsService.GetDemandForecastAsync(30, formattedStartDate, formattedEndDate);
            
            var viewModel = new SalesViewModel
            {
                SalesAnalytics = salesData,
                ExtendedAnalytics = extendedAnalytics,
                AbcAnalysis = abcAnalysis,
                SalesForecast = salesForecast
            };
            
            return Partial("Shared/Analytics/Sales/_Sales", viewModel);
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
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var ordersAnalytics = await _analyticsService.GetOrdersAnalyticsAsync(formattedStartDate, formattedEndDate);
            var ordersTrend = await _analyticsService.GetSalesTrendAsync(
                formattedStartDate ?? DateTime.UtcNow.AddDays(-30).Date,
                formattedEndDate ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1));
                
            var viewModel = new OrdersViewModel
            {
                OrdersAnalytics = ordersAnalytics,
                OrdersTrend = ordersTrend
            };
            
            return Partial("Shared/Analytics/Orders/_Orders", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке представления заказов");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetOrdersDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var ordersAnalytics = await _analyticsService.GetOrdersAnalyticsAsync(formattedStartDate, formattedEndDate);
            return new JsonResult(ordersAnalytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных заказов");
            return StatusCode(500);
        }
    }
    
    public async Task<IActionResult> OnGetTopProductsDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var topProducts = await _analyticsService.GetTopProductsAsync(
                10,
                formattedStartDate ?? DateTime.UtcNow.AddDays(-30).Date,
                formattedEndDate ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1));
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
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var salesData = await _analyticsService.GetSalesAnalyticsAsync(formattedStartDate, formattedEndDate);
            return new JsonResult(salesData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных продаж");
            return StatusCode(500);
        }
    }
    
    public async Task<IActionResult> OnGetForecastDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var forecastData = await _analyticsService.GetDemandForecastAsync(30,
                formattedStartDate ?? DateTime.UtcNow.AddDays(-30).Date,
                formattedEndDate ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1));
            return new JsonResult(forecastData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных прогноза");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetCategorySalesDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var categorySales = await _analyticsService.GetCategorySalesAsync(formattedStartDate, formattedEndDate);
            return new JsonResult(categorySales);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных продаж по категориям");
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnGetAbcAnalysisDataAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var abcAnalysis = await _analyticsService.GetProductAbcAnalysisAsync(formattedStartDate, formattedEndDate);
            return new JsonResult(abcAnalysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных ABC анализа");
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
            var (formattedStartDate, formattedEndDate) = FormatDateRange(startDate, endDate);
            var trendData = await _analyticsService.GetSalesTrendAsync(
                formattedStartDate ?? DateTime.UtcNow.AddDays(-30).Date,
                formattedEndDate ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1),
                interval);
            return new JsonResult(trendData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных тренда");
            return StatusCode(500);
        }
    }
    
    private (DateTime? startDate, DateTime? endDate) FormatDateRange(DateTime? startDate, DateTime? endDate)
    {
        return (
            startDate?.Date,
            endDate?.Date.AddDays(1).AddTicks(-1)
        );
    }
} 