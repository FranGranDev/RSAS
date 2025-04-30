using Application.DTOs;
using Server.Models;

namespace Server.Services.Repository
{
    public interface ISaleRepository : IRepository<Sale, int>
    {
        // Базовые операции с деталями
        Task<Sale> GetWithDetailsAsync(int id);
        Task<IEnumerable<Sale>> GetAllWithDetailsAsync();

        // Фильтрация
        Task<IEnumerable<Sale>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Sale>> GetByClientAsync(string clientPhone);
        Task<IEnumerable<Sale>> GetByProductAsync(int productId);
        Task<IEnumerable<Sale>> GetByCategoryAsync(string category);

        // Аналитика
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalSalesCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageSaleAmountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(
            int count = 10,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval);

        // Проверки
        Task<bool> ExistsByOrderIdAsync(int orderId);

        // Расширенная аналитика
        Task<decimal> GetSalesConversionRateAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<TimeSpan> GetAverageOrderProcessingTimeAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<StockEfficiencyResultDto>> GetStockEfficiencyAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<SeasonalityResultDto>> GetSeasonalityAsync(
            int years = 3);
        Task<IEnumerable<SalesForecastResultDto>> GetSalesForecastAsync(
            int days = 30);
        Task<KpiDto> GetKpiAsync(DateTime? startDate = null, DateTime? endDate = null);

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