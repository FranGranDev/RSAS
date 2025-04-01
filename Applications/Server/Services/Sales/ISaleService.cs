using Application.DTOs;

namespace Application.Services.Sales
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<SaleDto> GetSaleByIdAsync(int id);
        Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto);
        Task<SaleDto> UpdateSaleAsync(int id, UpdateSaleDto updateSaleDto);
        Task DeleteSaleAsync(int id);
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SaleDto>> GetSalesByStockIdAsync(int stockId);
    }
}