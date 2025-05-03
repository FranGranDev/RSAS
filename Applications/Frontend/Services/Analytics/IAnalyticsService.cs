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
} 