using Application.DTOs;

namespace Frontend.Services.Api;

public class AnalyticsService : IAnalyticsService
{
    private readonly IApiService _apiService;
    private const string BaseUri = "api/sales";

    public AnalyticsService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<DashboardAnalyticsDto>($"{BaseUri}/dashboard{queryParams}");
    }

    public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<SalesAnalyticsDto>($"{BaseUri}/analytics{queryParams}");
    }

    public async Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<OrdersAnalyticsDto>($"{BaseUri}/orders-analytics{queryParams}");
    }

    public async Task<ExtendedSalesAnalyticsDto> GetExtendedAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<ExtendedSalesAnalyticsDto>($"{BaseUri}/extended{queryParams}");
    }

    public async Task<KpiDto> GetKpiAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<KpiDto>($"{BaseUri}/kpi{queryParams}");
    }

    public async Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(int count = 10, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<TopProductResultDto>>($"{BaseUri}/top-products?count={count}{queryParams}");
    }

    public async Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<CategorySalesResultDto>>($"{BaseUri}/category-sales{queryParams}");
    }

    public async Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(DateTime startDate, DateTime endDate, string interval = "1d")
    {
        return await _apiService.GetAsync<IEnumerable<SalesTrendResultDto>>(
            $"{BaseUri}/trend?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&interval={interval}");
    }

    public async Task<IEnumerable<CategoryForecastDto>> GetCategoryForecastAsync(int days = 30, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<CategoryForecastDto>>($"{BaseUri}/forecast/categories?days={days}{queryParams}");
    }

    public async Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(int days = 30, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<DemandForecastDto>>($"{BaseUri}/forecast/demand?days={days}{queryParams}");
    }

    public async Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(int years = 3, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.GetAsync<IEnumerable<SeasonalityImpactDto>>($"{BaseUri}/seasonality?years={years}{queryParams}");
    }

    public async Task<ReportDto> GenerateReportAsync(ReportType type, ReportFormat format, DateTime? startDate = null, DateTime? endDate = null, ReportFormattingSettings? formattingSettings = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.PostAsync<ReportDto, ReportFormattingSettings>(
            $"{BaseUri}/report?type={type}&format={format}{queryParams}",
            formattingSettings ?? new ReportFormattingSettings());
    }

    public async Task<ReportDto> GenerateExtendedReportAsync(ReportType type, ReportFormat format, DateTime? startDate = null, DateTime? endDate = null, ReportFormattingSettings? formattingSettings = null)
    {
        var queryParams = BuildDateRangeQueryParams(startDate, endDate);
        return await _apiService.PostAsync<ReportDto, ReportFormattingSettings>(
            $"{BaseUri}/report/extended?type={type}&format={format}{queryParams}",
            formattingSettings ?? new ReportFormattingSettings());
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