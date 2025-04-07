using Application.DTOs;
using Server.Models;

namespace Server.Services.Sales
{
    public interface ISaleService
    {
        // Базовые операции
        Task<SaleDto> GetByIdAsync(int id);
        Task<IEnumerable<SaleDto>> GetAllAsync();
        Task<SaleDto> CreateFromOrderAsync(int orderId);
        
        // Получение с деталями
        Task<SaleDto> GetWithDetailsAsync(int id);
        Task<IEnumerable<SaleDto>> GetAllWithDetailsAsync();

        // Фильтрация
        Task<IEnumerable<SaleDto>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SaleDto>> GetByClientAsync(string clientPhone);
        Task<IEnumerable<SaleDto>> GetByProductAsync(int productId);
        Task<IEnumerable<SaleDto>> GetByCategoryAsync(string category);

        // Аналитика
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalCostAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalSalesCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageSaleAmountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<TopProductDto>> GetTopProductsAsync(
            int count = 10,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<SalesTrendDto>> GetSalesTrendAsync(
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval);

        // Комплексная аналитика
        Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ReportDto> GenerateReportAsync(ReportType type, ReportFormat format, DateTime? startDate = null, DateTime? endDate = null);

        // Проверки
        Task<bool> ExistsByOrderIdAsync(int orderId);
    }
}