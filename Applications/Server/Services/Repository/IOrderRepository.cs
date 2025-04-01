using Application.Models;

namespace Server.Services.Repository
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        Task<Order> GetWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetByStockIdAsync(int stockId);
        Task<IEnumerable<Order>> GetByStateAsync(Order.States state);
        Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> ExistsByUserIdAsync(string userId);
    }
}