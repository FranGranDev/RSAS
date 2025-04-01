using Application.Areas.Identity.Data;
using Application.Model.Stocks;
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
    }
} 