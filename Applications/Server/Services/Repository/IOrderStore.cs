using Application.Model.Orders;
using Application.Model.Stocks;
using Application.ViewModel.Catalog;

namespace Application.Services.Repository
{
    public interface IOrderStore : IStore<Order>
    {
        Task<IEnumerable<Order>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<Order>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<Order>> GetByStatusAsync(Order.States status);
        Task<Order> CreateOrderAsync(Order order, IEnumerable<CatalogItemViewModel> items, Delivery delivery);
        Task ExecuteOrderAsync(Order order, Stock stock);
        Task CompleteOrderAsync(Order order);
    }
}