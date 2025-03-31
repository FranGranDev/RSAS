using Application.Model.Stocks;

namespace Application.Services
{
    public interface IStockStore
    {
        public IQueryable<Stock> All { get; }
        public Stock Get(int id);
        public void Save(Stock stock);
        public void Delete(int id);
    }
}
