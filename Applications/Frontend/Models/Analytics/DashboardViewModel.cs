using Application.DTOs;

namespace Frontend.Models.Analytics;

public class DashboardViewModel
{
    public DashboardAnalyticsDto Dashboard { get; set; }
    public IEnumerable<SalesTrendResultDto> SalesTrend { get; set; }
}