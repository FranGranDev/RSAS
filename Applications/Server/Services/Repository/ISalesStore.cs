using Application.Model.Orders;
using Application.Model.Sales;

namespace Application.Services.Repository
{
    public interface ISalesStore : IStore<Sale>
    {
        Task<IEnumerable<Sale>> GetByStockIdAsync(int stockId);
        Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task CreateSaleAsync(Order order);
    }
}