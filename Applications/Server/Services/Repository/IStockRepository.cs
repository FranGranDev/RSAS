using Application.Models;
using Server.Services.Repository;

namespace Application.Services.Repository
{
    public interface IStockRepository : IRepository<Stock, int>
    {
        Task<Stock> GetByNameAsync(string name);
        Task<Stock> GetByAddressAsync(string address);
        Task<IEnumerable<Stock>> GetByCityAsync(string city);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByAddressAsync(string address);
        Task<Stock> GetWithStockProductsAsync(int id);
        Task<IEnumerable<Stock>> GetAllWithStockProductsAsync();
        Task<StockProducts> GetStockProductAsync(int stockId, int productId);
        Task UpdateStockProductAsync(StockProducts stockProduct);
    }
}