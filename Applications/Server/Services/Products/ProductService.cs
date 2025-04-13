using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Application.Services.Repository;
using AutoMapper;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllWithStockProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetWithStockProductsAsync(id);
            if (product == null)
            {
                throw new BusinessException($"Товар с ID {id} не найден");
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto createProductDto)
        {
            // Проверяем, не существует ли уже товар с таким названием
            if (await _productRepository.ExistsByNameAsync(createProductDto.Name))
            {
                throw new BusinessException("Товар с таким названием уже существует");
            }

            // Проверяем, не существует ли уже товар с таким штрих-кодом
            if (await _productRepository.ExistsByBarcodeAsync(createProductDto.Barcode))
            {
                throw new BusinessException("Товар с таким штрих-кодом уже существует");
            }

            var product = _mapper.Map<Product>(createProductDto);
            await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, ProductDto updateProductDto)
        {
            var product = await _productRepository.GetWithStockProductsAsync(id);
            if (product == null)
            {
                throw new BusinessException($"Товар с ID {id} не найден");
            }

            // Проверяем, не занято ли новое название другим товаром
            if (product.Name != updateProductDto.Name &&
                await _productRepository.ExistsByNameAsync(updateProductDto.Name))
            {
                throw new BusinessException("Товар с таким названием уже существует");
            }

            // Проверяем, не занят ли новый штрих-код другим товаром
            if (product.Barcode != updateProductDto.Barcode &&
                await _productRepository.ExistsByBarcodeAsync(updateProductDto.Barcode))
            {
                throw new BusinessException("Товар с таким штрих-кодом уже существует");
            }

            _mapper.Map(updateProductDto, product);
            await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetWithStockProductsAsync(id);
            if (product == null)
            {
                throw new BusinessException($"Товар с ID {id} не найден");
            }

            // Проверяем, нет ли связанных записей в StockProducts
            if (product.StockProducts != null && product.StockProducts.Any())
            {
                throw new BusinessException("Невозможно удалить товар, так как он есть на складах");
            }

            await _productRepository.DeleteAsync(product);
        }

        public async Task<ProductDto> GetProductByNameAsync(string name)
        {
            var product = await _productRepository.GetByNameAsync(name);
            if (product == null)
            {
                throw new BusinessException($"Товар с названием {name} не найден");
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> GetProductByBarcodeAsync(string barcode)
        {
            var product = await _productRepository.GetByBarcodeAsync(barcode);
            if (product == null)
            {
                throw new BusinessException($"Товар со штрих-кодом {barcode} не найден");
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var products = await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _productRepository.ExistsByNameAsync(name);
        }

        public async Task<bool> ExistsByBarcodeAsync(string barcode)
        {
            return await _productRepository.ExistsByBarcodeAsync(barcode);
        }
    }
}