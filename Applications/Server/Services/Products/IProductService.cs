using Application.DTOs;

namespace Application.Services.Products
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsByStockIdAsync(int stockId);
    }
}