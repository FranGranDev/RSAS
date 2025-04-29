using Application.DTOs;

namespace Frontend.Services.Api;

public interface IAnalyticsService
{
    Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ExtendedSalesAnalyticsDto> GetExtendedAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<KpiDto> GetKpiAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(int count = 10, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(DateTime startDate, DateTime endDate, string interval = "1d");
    Task<IEnumerable<CategoryForecastDto>> GetCategoryForecastAsync(int days = 30, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(int days = 30, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(int years = 3, DateTime? startDate = null, DateTime? endDate = null);
    Task<ReportDto> GenerateReportAsync(ReportType type, ReportFormat format, DateTime? startDate = null, DateTime? endDate = null, ReportFormattingSettings? formattingSettings = null);
    Task<ReportDto> GenerateExtendedReportAsync(ReportType type, ReportFormat format, DateTime? startDate = null, DateTime? endDate = null, ReportFormattingSettings? formattingSettings = null);
} 