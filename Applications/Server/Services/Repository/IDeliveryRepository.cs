using Application.Models;

namespace Server.Services.Repository
{
    public interface IDeliveryRepository : IRepository<Delivery, int>
    {
        Task<Delivery> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Delivery>> GetByStatusAsync(string status);
        Task<IEnumerable<Delivery>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> ExistsByOrderIdAsync(int orderId);
    }
}