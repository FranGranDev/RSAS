using Application.Areas.Identity.Data;
using Application.DTOs;
using Application.Exceptions;
using Application.Model.Stocks;
using Application.Services.Repository;
using Server.Services.Repository;
using AutoMapper;

namespace Application.Services.Stocks
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        public StockService(
            IStockRepository stockRepository,
            IMapper mapper)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            var stocks = await _stockRepository.GetAllWithStockProductsAsync();
            return _mapper.Map<IEnumerable<StockDto>>(stocks);
        }

        public async Task<StockDto> GetStockByIdAsync(int id)
        {
            var stock = await _stockRepository.GetWithStockProductsAsync(id);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {id} не найден");
            }

            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> CreateStockAsync(CreateStockDto createStockDto)
        {
            // Проверяем, не существует ли уже склад с таким названием
            if (await _stockRepository.ExistsByNameAsync(createStockDto.Name))
            {
                throw new BusinessException("Склад с таким названием уже существует");
            }

            // Проверяем, не существует ли уже склад по такому адресу
            if (await _stockRepository.ExistsByAddressAsync(createStockDto.Address))
            {
                throw new BusinessException("Склад по такому адресу уже существует");
            }

            var stock = _mapper.Map<Stock>(createStockDto);
            await _stockRepository.AddAsync(stock);

            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> UpdateStockAsync(int id, UpdateStockDto updateStockDto)
        {
            var stock = await _stockRepository.GetWithStockProductsAsync(id);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {id} не найден");
            }

            // Проверяем, не существует ли уже склад с таким названием
            if (stock.Name != updateStockDto.Name && await _stockRepository.ExistsByNameAsync(updateStockDto.Name))
            {
                throw new BusinessException("Склад с таким названием уже существует");
            }

            // Проверяем, не существует ли уже склад по такому адресу
            if (stock.Address != updateStockDto.Address && await _stockRepository.ExistsByAddressAsync(updateStockDto.Address))
            {
                throw new BusinessException("Склад по такому адресу уже существует");
            }

            _mapper.Map(updateStockDto, stock);
            await _stockRepository.UpdateAsync(stock);

            return _mapper.Map<StockDto>(stock);
        }

        public async Task DeleteStockAsync(int id)
        {
            var stock = await _stockRepository.GetWithStockProductsAsync(id);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {id} не найден");
            }

            await _stockRepository.DeleteAsync(stock);
        }

        public async Task<StockDto> GetStockByNameAsync(string name)
        {
            var stock = await _stockRepository.GetByNameAsync(name);
            if (stock == null)
            {
                throw new BusinessException($"Склад с названием {name} не найден");
            }

            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> GetStockByAddressAsync(string address)
        {
            var stock = await _stockRepository.GetByAddressAsync(address);
            if (stock == null)
            {
                throw new BusinessException($"Склад по адресу {address} не найден");
            }

            return _mapper.Map<StockDto>(stock);
        }

        public async Task<IEnumerable<StockDto>> GetStocksByCityAsync(string city)
        {
            var stocks = await _stockRepository.GetByCityAsync(city);
            return _mapper.Map<IEnumerable<StockDto>>(stocks);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _stockRepository.ExistsByNameAsync(name);
        }

        public async Task<bool> ExistsByAddressAsync(string address)
        {
            return await _stockRepository.ExistsByAddressAsync(address);
        }

        public async Task<IEnumerable<StockProductDto>> GetStockProductsAsync(int stockId)
        {
            var stock = await _stockRepository.GetWithStockProductsAsync(stockId);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {stockId} не найден");
            }

            return _mapper.Map<IEnumerable<StockProductDto>>(stock.StockProducts);
        }

        public async Task<StockProductDto> UpdateStockProductQuantityAsync(int stockId, int productId, int quantity)
        {
            var stock = await _stockRepository.GetWithStockProductsAsync(stockId);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {stockId} не найден");
            }

            var stockProduct = stock.StockProducts.FirstOrDefault(sp => sp.ProductId == productId);
            if (stockProduct == null)
            {
                throw new BusinessException($"Товар с ID {productId} не найден на складе");
            }

            if (quantity < 0)
            {
                throw new BusinessException("Количество товара не может быть отрицательным");
            }

            stockProduct.Quantity = quantity;
            await _stockRepository.UpdateAsync(stock);

            return _mapper.Map<StockProductDto>(stockProduct);
        }
    }
}