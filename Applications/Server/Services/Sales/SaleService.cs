using Application.DTOs;
using Application.Exceptions;
using Application.Model.Sales;
using Application.Services.Repository;
using AutoMapper;

namespace Application.Services.Sales
{
    public class SaleService : ISaleService
    {
        private readonly IMapper _mapper;
        private readonly ISalesStore _salesStore;
        private readonly IStockProductsStore _stockProductsStore;

        public SaleService(
            ISalesStore salesStore,
            IStockProductsStore stockProductsStore,
            IMapper mapper)
        {
            _salesStore = salesStore;
            _stockProductsStore = stockProductsStore;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _salesStore.GetAllAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<SaleDto> GetSaleByIdAsync(int id)
        {
            var sale = await _salesStore.GetByIdAsync(id);
            if (sale == null)
            {
                throw new BusinessException($"Продажа с ID {id} не найдена");
            }

            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto)
        {
            // Проверяем наличие товаров на складе
            foreach (var product in createSaleDto.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    createSaleDto.StockId, product.ProductId);

                if (stockProduct == null || stockProduct.Quantity < product.Quantity)
                {
                    throw new BusinessException($"Недостаточно товара {product.ProductId} на складе");
                }
            }

            var sale = _mapper.Map<Sale>(createSaleDto);
            sale.SaleDate = DateTime.UtcNow;

            // Уменьшаем количество товаров на складе
            foreach (var product in sale.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    sale.StockId, product.ProductId);

                stockProduct.Quantity -= product.Quantity;
                await _stockProductsStore.SaveAsync(stockProduct);
            }

            await _salesStore.SaveAsync(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<SaleDto> UpdateSaleAsync(int id, UpdateSaleDto updateSaleDto)
        {
            var sale = await _salesStore.GetByIdAsync(id);
            if (sale == null)
            {
                throw new BusinessException($"Продажа с ID {id} не найдена");
            }

            // Возвращаем товары на склад
            foreach (var product in sale.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    sale.StockId, product.ProductId);

                stockProduct.Quantity += product.Quantity;
                await _stockProductsStore.SaveAsync(stockProduct);
            }

            // Проверяем наличие товаров для обновленной продажи
            foreach (var product in updateSaleDto.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    updateSaleDto.StockId, product.ProductId);

                if (stockProduct == null || stockProduct.Quantity < product.Quantity)
                {
                    throw new BusinessException($"Недостаточно товара {product.ProductId} на складе");
                }
            }

            _mapper.Map(updateSaleDto, sale);
            sale.SaleDate = DateTime.UtcNow;

            // Уменьшаем количество товаров на складе для обновленной продажи
            foreach (var product in sale.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    sale.StockId, product.ProductId);

                stockProduct.Quantity -= product.Quantity;
                await _stockProductsStore.SaveAsync(stockProduct);
            }

            await _salesStore.SaveAsync(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _salesStore.GetByIdAsync(id);
            if (sale == null)
            {
                throw new BusinessException($"Продажа с ID {id} не найдена");
            }

            // Возвращаем товары на склад
            foreach (var product in sale.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    sale.StockId, product.ProductId);

                stockProduct.Quantity += product.Quantity;
                await _stockProductsStore.SaveAsync(stockProduct);
            }

            await _salesStore.DeleteAsync(id);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _salesStore.GetAllAsync();
            var filteredSales = sales.Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);
            return _mapper.Map<IEnumerable<SaleDto>>(filteredSales);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByStockIdAsync(int stockId)
        {
            var sales = await _salesStore.GetAllAsync();
            var filteredSales = sales.Where(s => s.StockId == stockId);
            return _mapper.Map<IEnumerable<SaleDto>>(filteredSales);
        }
    }
}