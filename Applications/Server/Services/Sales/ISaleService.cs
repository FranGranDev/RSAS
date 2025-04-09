using Application.DTOs;
using Server.Models;

namespace Server.Services.Sales
{
    public interface ISaleService
    {
        // Базовые операции с продажами
        Task<SaleDto> GetByIdAsync(int id);
        Task<IEnumerable<SaleDto>> GetAllAsync();
        Task<SaleDto> CreateFromOrderAsync(int orderId);
        Task<bool> ExistsByOrderIdAsync(int orderId);

        // Фильтрация продаж
        Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SaleDto>> GetByClientAsync(string clientPhone);
        Task<IEnumerable<SaleDto>> GetByProductAsync(int productId);
        Task<IEnumerable<SaleDto>> GetByCategoryAsync(string category);

        // Базовая аналитика
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalCostAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalSalesCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageSaleAmountAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Аналитика по продуктам
        Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(
            int count = 10,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);

        // Аналитика по времени
        Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval);

        // Комплексная аналитика
        Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Расширенная аналитика
        Task<ExtendedSalesAnalyticsDto> GetExtendedAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<KpiDto> GetKpiAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Отчеты
        /// <summary>
        /// Генерация отчета
        /// </summary>
        Task<ReportDto> GenerateReportAsync(
            ReportType type,
            ReportFormat format,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ReportFormattingSettings? formattingSettings = null,
            string? userId = null,
            string? userName = null);

        /// <summary>
        /// Генерация расширенного отчета
        /// </summary>
        Task<ReportDto> GenerateExtendedReportAsync(
            ReportType type,
            ReportFormat format,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ReportFormattingSettings? formattingSettings = null,
            string? userId = null,
            string? userName = null);

        // Прогнозирование
        Task<IEnumerable<CategoryForecastDto>> GetCategoryForecastAsync(
            int days = 30,
            DateTime? startDate = null,
            DateTime? endDate = null);
            
        Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(
            int days = 30,
            DateTime? startDate = null,
            DateTime? endDate = null);
            
        Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(
            int years = 3,
            DateTime? startDate = null,
            DateTime? endDate = null);
    }
}