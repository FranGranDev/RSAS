using Application.DTOs;
using Application.Exceptions;
using Application.Model.Stocks;
using Application.Services.Repository;
using AutoMapper;

namespace Application.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductsStore _productsStore;
        private readonly IStockProductsStore _stockProductsStore;

        public ProductService(
            IProductsStore productsStore,
            IStockProductsStore stockProductsStore,
            IMapper mapper)
        {
            _productsStore = productsStore;
            _stockProductsStore = stockProductsStore;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productsStore.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productsStore.GetByIdAsync(id);
            if (product == null)
            {
                throw new BusinessException($"Товар с ID {id} не найден");
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            await _productsStore.SaveAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productsStore.GetByIdAsync(id);
            if (product == null)
            {
                throw new BusinessException($"Товар с ID {id} не найден");
            }

            _mapper.Map(updateProductDto, product);
            await _productsStore.SaveAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productsStore.GetByIdAsync(id);
            if (product == null)
            {
                throw new BusinessException($"Товар с ID {id} не найден");
            }

            // Проверяем, нет ли связанных записей в StockProducts
            var stockProducts = await _stockProductsStore.GetByProductIdAsync(id);
            if (stockProducts.Any())
            {
                throw new BusinessException("Невозможно удалить товар, так как он есть на складах");
            }

            await _productsStore.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByStockIdAsync(int stockId)
        {
            var stockProducts = await _stockProductsStore.GetByStockIdAsync(stockId);
            var products = stockProducts.Select(sp => sp.Product);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}