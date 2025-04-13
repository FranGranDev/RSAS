using Application.DTOs;

namespace Application.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(ProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id, ProductDto updateProductDto);
        Task DeleteProductAsync(int id);
        Task<ProductDto> GetProductByNameAsync(string name);
        Task<ProductDto> GetProductByBarcodeAsync(string barcode);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByBarcodeAsync(string barcode);
    }
}