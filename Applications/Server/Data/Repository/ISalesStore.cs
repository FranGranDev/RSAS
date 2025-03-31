using Application.Model.Orders;

namespace Application.Data.Repository
{
    public interface ISalesStore : IStore<Sale>
    {
        Task<Sale> CreateSale(Order order);
    }
} 