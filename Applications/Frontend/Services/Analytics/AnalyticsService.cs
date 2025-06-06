using Application.DTOs;
using Frontend.Models.Analytics;

namespace Frontend.Services.Api;

public class AnalyticsService : IAnalyticsService
{
    private readonly IApiService _apiService;
    private const string BaseUri = "api/sales";

    public AnalyticsService(IApiService apiService)
    {
        _apiService = apiService;
    }

    //Pages
    public async Task<DashboardViewModel> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        var dashboard = await _apiService.GetAsync<DashboardAnalyticsDto>($"{BaseUri}/dashboard{queryParams}");
        var salesTrend = await GetSalesTrendAsync(
            startDate ?? DateTime.UtcNow.AddDays(-30),
            endDate ?? DateTime.UtcNow);
            
        return new DashboardViewModel()
        {
            Dashboard = dashboard,
            SalesTrend = salesTrend
        };
    }
    
    //Special
    public async Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(int count, DateTime startDate, DateTime endDate)
    {
        return await _apiService.GetAsync<IEnumerable<TopProductResultDto>>(
            $"{BaseUri}/top-products?count={count}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
    }
    
    public async Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(DateTime startDate, DateTime endDate, string interval = "1d")
    {
        return await _apiService.GetAsync<IEnumerable<SalesTrendResultDto>>(
            $"{BaseUri}/trend?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&interval={interval}");
    }

    //Sales
    public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<SalesAnalyticsDto>($"{BaseUri}/analytics{queryParams}");
    }

    public async Task<ExtendedSalesAnalyticsDto> GetExtendedSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<ExtendedSalesAnalyticsDto>($"{BaseUri}/extended{queryParams}");
    }

    public async Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<CategorySalesResultDto>>($"{BaseUri}/category-sales{queryParams}");
    }

    public async Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(int years = 3, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<SeasonalityImpactDto>>($"{BaseUri}/seasonality/years/{years}{queryParams}");
    }

    public async Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(int days = 30, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<DemandForecastDto>>($"{BaseUri}/forecast/demand/days/{days}{queryParams}");
    }

    public async Task<IEnumerable<ProductAbcAnalysisDto>> GetProductAbcAnalysisAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<ProductAbcAnalysisDto>>($"{BaseUri}/abc-analysis{queryParams}");
    }

    public async Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<OrdersAnalyticsDto>($"{BaseUri}/orders-analytics{queryParams}");
    }

    private static string BuildDateRangeQueryParams(DateTime? startDate, DateTime? endDate)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue)
            queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue)
            queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
        return queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
    }
} 