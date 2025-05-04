using Application.DTOs;

namespace Frontend.Models.Analytics;

public class SalesViewModel
{
    public SalesAnalyticsDto SalesAnalytics { get; set; }
    public ExtendedSalesAnalyticsDto ExtendedAnalytics { get; set; }
    public IEnumerable<ProductAbcAnalysisDto> AbcAnalysis { get; set; }
} 