using Application.Areas.Identity.Data;
using Application.Model.Stocks;
using Server.Services.Repository;

namespace Application.Services.Repository
{
    public interface IProductRepository : IRepository<Product, int>
    {
        Task<Product> GetByNameAsync(string name);
        Task<Product> GetByBarcodeAsync(string barcode);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByBarcodeAsync(string barcode);
        Task<Product> GetWithStockProductsAsync(int id);
        Task<IEnumerable<Product>> GetAllWithStockProductsAsync();
    }
} 