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
        return new DashboardViewModel()
        {
            Dashboard = await _apiService.GetAsync<DashboardAnalyticsDto>($"{BaseUri}/dashboard{queryParams}"),
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