using Application.DTOs;
using Application.Exceptions;
using Application.Model.Stocks;
using Application.Services.Repository;
using AutoMapper;

namespace Application.Services.Stocks
{
    public class StockService : IStockService
    {
        private readonly IMapper _mapper;
        private readonly IStockProductsStore _stockProductsStore;
        private readonly IStockStore _stockStore;

        public StockService(
            IStockStore stockStore,
            IStockProductsStore stockProductsStore,
            IMapper mapper)
        {
            _stockStore = stockStore;
            _stockProductsStore = stockProductsStore;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            var stocks = await _stockStore.GetAllAsync();
            return _mapper.Map<IEnumerable<StockDto>>(stocks);
        }

        public async Task<StockDto> GetStockByIdAsync(int id)
        {
            var stock = await _stockStore.GetByIdAsync(id);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {id} не найден");
            }

            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> CreateStockAsync(CreateStockDto createStockDto)
        {
            var stock = _mapper.Map<Stock>(createStockDto);
            await _stockStore.SaveAsync(stock);
            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> UpdateStockAsync(int id, UpdateStockDto updateStockDto)
        {
            var stock = await _stockStore.GetByIdAsync(id);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {id} не найден");
            }

            _mapper.Map(updateStockDto, stock);
            await _stockStore.SaveAsync(stock);
            return _mapper.Map<StockDto>(stock);
        }

        public async Task DeleteStockAsync(int id)
        {
            var stock = await _stockStore.GetByIdAsync(id);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {id} не найден");
            }

            // Проверяем, нет ли товаров на складе
            var stockProducts = await _stockProductsStore.GetByStockIdAsync(id);
            if (stockProducts.Any())
            {
                throw new BusinessException("Невозможно удалить склад, так как на нем есть товары");
            }

            await _stockStore.DeleteAsync(id);
        }

        public async Task<IEnumerable<StockProductDto>> GetStockProductsAsync(int stockId)
        {
            var stock = await _stockStore.GetByIdAsync(stockId);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {stockId} не найден");
            }

            var stockProducts = await _stockProductsStore.GetByStockIdAsync(stockId);
            return _mapper.Map<IEnumerable<StockProductDto>>(stockProducts);
        }

        public async Task<StockProductDto> UpdateStockProductQuantityAsync(int stockId, int productId, int quantity)
        {
            var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(stockId, productId);
            if (stockProduct == null)
            {
                throw new BusinessException($"Товар с ID {productId} не найден на складе {stockId}");
            }

            if (quantity < 0)
            {
                throw new BusinessException("Количество товара не может быть отрицательным");
            }

            stockProduct.Quantity = quantity;
            await _stockProductsStore.SaveAsync(stockProduct);
            return _mapper.Map<StockProductDto>(stockProduct);
        }
    }
}