using Application.Model.Orders;
using Application.Model.Stocks;
using Application.ViewModel.Catalog;

namespace Application.Services
{
    public interface IOrderStore
    {
        public IQueryable<Order> All { get; }
        public Order Get(int id);
        public void Save(Order order);
        public void Delete(Order order);


        public Task<Order> CreateOrder(Order order, IEnumerable<CatalogItemViewModel> items, Delivery delivery);
        public void ExecuteOrder(Order order, Stock stock);
        public void CompleteOrder(Order order);
    }
}
