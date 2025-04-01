using Application.Model.Stocks;

namespace Application.Services.Repository
{
    public interface IStockProductsStore : IStore<StockProducts>
    {
        Task<IEnumerable<StockProducts>> GetByStockIdAsync(int stockId);
        Task<IEnumerable<StockProducts>> GetByProductIdAsync(int productId);
        Task<StockProducts> GetByStockAndProductIdAsync(int stockId, int productId);
    }
}