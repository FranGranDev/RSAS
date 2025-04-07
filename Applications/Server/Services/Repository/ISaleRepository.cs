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
        Task<decimal> GetTotalCostAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalSalesCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageSaleAmountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<(string ProductName, int SalesCount, decimal Revenue)>> GetTopProductsAsync(
            int count = 10,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<(string Category, int SalesCount, decimal Revenue, decimal Share)>> GetCategorySalesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<IEnumerable<(DateTime Date, int SalesCount, decimal Revenue)>> GetSalesTrendAsync(
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval);

        // Проверки
        Task<bool> ExistsByOrderIdAsync(int orderId);
    }
}