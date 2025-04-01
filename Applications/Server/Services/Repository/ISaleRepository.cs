using Application.Model.Sales;

namespace Server.Services.Repository
{
    public interface ISaleRepository : IRepository<Sale, int>
    {
        Task<Sale> GetWithDetailsAsync(int id);
        Task<IEnumerable<Sale>> GetAllWithDetailsAsync();
        Task<IEnumerable<Sale>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Sale>> GetByStockIdAsync(int stockId);
        Task<IEnumerable<Sale>> GetByStatusAsync(SaleStatus status);
        Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> ExistsByOrderIdAsync(int orderId);
    }
} 