using Application.Model.Stocks;

namespace Application.Services
{
    public interface IStockProductsStore
    {
        public IQueryable<StockProducts> All { get; }
        public void Save(StockProducts stockProducts);
        public void Delete(StockProducts stockProduct);
    }
}
