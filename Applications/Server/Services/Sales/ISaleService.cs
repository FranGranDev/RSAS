using Application.DTOs;
using Application.Model.Sales;

namespace Server.Services.Sales
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<SaleDto> GetSaleByIdAsync(int id);
        Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto);
        Task<SaleDto> UpdateSaleAsync(int id, UpdateSaleDto updateSaleDto);
        Task DeleteSaleAsync(int id);
        Task<IEnumerable<SaleDto>> GetSalesByOrderIdAsync(int orderId);
        Task<IEnumerable<SaleDto>> GetSalesByStockIdAsync(int stockId);
        Task<IEnumerable<SaleDto>> GetSalesByStatusAsync(SaleStatus status);
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<SaleDto> CompleteSaleAsync(int id);
        Task<SaleDto> CancelSaleAsync(int id);
    }
}