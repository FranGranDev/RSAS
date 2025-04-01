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
        Task<IEnumerable<StockProductDto>> GetStockProductsAsync(int stockId);
        Task<StockProductDto> UpdateStockProductQuantityAsync(int stockId, int productId, int quantity);
    }
}