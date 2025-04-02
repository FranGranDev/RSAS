using Application.DTOs;

namespace Application.Services.Stocks
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAllStocksAsync();
        Task<StockDto> GetStockByIdAsync(int id);
        Task<StockDto> CreateStockAsync(CreateStockDto createStockDto);
        Task<StockDto> UpdateStockAsync(int id, UpdateStockDto updateStockDto);
        Task DeleteStockAsync(int id);
        Task<StockDto> GetStockByNameAsync(string name);
        Task<StockDto> GetStockByAddressAsync(string address);
        Task<IEnumerable<StockDto>> GetStocksByCityAsync(string city);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByAddressAsync(string address);
        Task<IEnumerable<StockProductDto>> GetStockProductsAsync(int stockId);
        Task<StockProductDto> UpdateStockProductQuantityAsync(int stockId, int productId, int quantity);
        
        // Новые методы для управления товарами на складе
        Task<StockProductDto> AddProductToStockAsync(int stockId, int productId, int quantity);
        Task RemoveProductFromStockAsync(int stockId, int productId);
        Task<StockProductDto> GetStockProductAsync(int stockId, int productId);
        Task<bool> HasProductOnStockAsync(int stockId, int productId);
        Task<int> GetProductQuantityOnStockAsync(int stockId, int productId);
    }
}