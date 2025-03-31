using Application.Model.Orders;
using Application.Model.Sales;
using Application.Model.Stocks;
using Application.ViewModel.Catalog;

namespace Application.Services
{
    public interface ISalesStore
    {
        public IQueryable<Sale> All { get; }
        public Sale Get(int id);
        public void Save(Sale order);


        public void CreateSale(Order order);
    }
}
