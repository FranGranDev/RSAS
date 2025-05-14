using Application.DTOs;
using Frontend.Models.Analytics;
using Frontend.Services.Api;
using Frontend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Analytics;

[Authorize(Roles = "Manager")]
public class PrintModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly IAnalyticsService _analyticsService;

    public PrintModel(IAuthService authService, IAnalyticsService analyticsService)
    {
        _authService = authService;
        _analyticsService = analyticsService;
    }

    public ReportInfoModel ReportInfo { get; set; }
    public string Section { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public DashboardViewModel DashboardData { get; set; }
    public SalesViewModel SalesData { get; set; }
    public OrdersViewModel OrdersData { get; set; }

    public async Task<IActionResult> OnGetAsync(string section, string startDate, string endDate)
    {
        try
        {
            // Получаем информацию о текущем пользователе
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Парсим даты
            if (DateTime.TryParse(startDate, out var parsedStartDate))
                StartDate = parsedStartDate;
            if (DateTime.TryParse(endDate, out var parsedEndDate))
                EndDate = parsedEndDate;

            // Формируем информацию об отчете
            ReportInfo = new ReportInfoModel
            {
                GeneratedAt = DateTime.Now,
                UserName = currentUser.Email,
                Period = $"{startDate} - {endDate}",
                Section = GetSectionName(section)
            };

            Section = section;

            // Загружаем данные в зависимости от выбранной вкладки
            switch (section)
            {
                case "dashboard":
                    DashboardData = await _analyticsService.GetDashboardAnalyticsAsync(StartDate, EndDate);
                    break;
                case "sales":
                    var salesAnalytics = await _analyticsService.GetSalesAnalyticsAsync(StartDate, EndDate);
                    var extendedAnalytics = await _analyticsService.GetExtendedSalesAnalyticsAsync(StartDate, EndDate);
                    var abcAnalysis = await _analyticsService.GetProductAbcAnalysisAsync(StartDate, EndDate);
                    var salesForecast = await _analyticsService.GetDemandForecastAsync(30, StartDate, EndDate);
                    SalesData = new SalesViewModel
                    {
                        SalesAnalytics = salesAnalytics,
                        ExtendedAnalytics = extendedAnalytics,
                        AbcAnalysis = abcAnalysis,
                        SalesForecast = salesForecast
                    };
                    break;
                case "orders":
                    var ordersAnalytics = await _analyticsService.GetOrdersAnalyticsAsync(StartDate, EndDate);
                    var ordersTrend = await _analyticsService.GetSalesTrendAsync(
                        StartDate ?? DateTime.UtcNow.AddDays(-30),
                        EndDate ?? DateTime.UtcNow);
                    OrdersData = new OrdersViewModel
                    {
                        OrdersAnalytics = ordersAnalytics,
                        OrdersTrend = ordersTrend
                    };
                    break;
            }

            return Page();
        }
        catch (Exception)
        {
            return RedirectToPage("/Error");
        }
    }

    private string GetSectionName(string section)
    {
        return section switch
        {
            "dashboard" => "Общая аналитика",
            "sales" => "Продажи",
            "orders" => "Заказы",
            "reports" => "Отчеты",
            _ => section
        };
    }
} 