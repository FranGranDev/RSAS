using Application.DTOs;

namespace Frontend.Models.Analytics;

public class OrdersViewModel
{
    public OrdersAnalyticsDto OrdersAnalytics { get; set; } = new();
    public IEnumerable<SalesTrendResultDto> OrdersTrend { get; set; } = new List<SalesTrendResultDto>();
} 