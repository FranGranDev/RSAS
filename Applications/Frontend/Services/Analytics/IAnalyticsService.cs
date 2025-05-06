using Application.DTOs;
using Frontend.Models.Analytics;

namespace Frontend.Services.Api;

public interface IAnalyticsService
{
    //Pages
    Task<DashboardViewModel> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    //Special
    Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(int count, DateTime startDate, DateTime endDate);
    Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(DateTime startDate, DateTime endDate, string interval = "1d");
    
    //Sales
    Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ExtendedSalesAnalyticsDto> GetExtendedSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(int years = 3, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(int days = 30, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<ProductAbcAnalysisDto>> GetProductAbcAnalysisAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    //Orders
    Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
} 