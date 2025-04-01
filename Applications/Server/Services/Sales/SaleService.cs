using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Application.Services.Repository;
using AutoMapper;
using Server.Services.Repository;

namespace Server.Services.Sales
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        public SaleService(
            ISaleRepository saleRepository,
            IOrderRepository orderRepository,
            IStockRepository stockRepository,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _orderRepository = orderRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _saleRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<SaleDto> GetSaleByIdAsync(int id)
        {
            var sale = await _saleRepository.GetWithDetailsAsync(id);
            if (sale == null)
                throw new BusinessException($"Продажа с ID {id} не найдена");

            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto)
        {
            // Проверяем существование заказа
            var order = await _orderRepository.GetWithDetailsAsync(createSaleDto.OrderId);
            if (order == null)
                throw new BusinessException($"Заказ с ID {createSaleDto.OrderId} не найден");

            if (order.State != Order.States.InProcess)
                throw new BusinessException("Можно создать продажу только для заказа в процессе");

            // Проверяем существование склада
            var stock = await _stockRepository.GetByIdAsync(createSaleDto.StockId);
            if (stock == null)
                throw new BusinessException($"Склад с ID {createSaleDto.StockId} не найден");

            // Проверяем, не существует ли уже продажа для этого заказа
            if (await _saleRepository.ExistsByOrderIdAsync(createSaleDto.OrderId))
                throw new BusinessException($"Продажа для заказа с ID {createSaleDto.OrderId} уже существует");

            var sale = _mapper.Map<Sale>(createSaleDto);
            sale.SaleDate = DateTime.UtcNow;
            sale.Status = SaleStatus.Processing;

            await _saleRepository.AddAsync(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<SaleDto> UpdateSaleAsync(int id, UpdateSaleDto updateSaleDto)
        {
            var sale = await _saleRepository.GetWithDetailsAsync(id);
            if (sale == null)
                throw new BusinessException($"Продажа с ID {id} не найдена");

            if (sale.Status == SaleStatus.Completed)
                throw new BusinessException("Нельзя изменить завершенную продажу");

            if (sale.Status == SaleStatus.Cancelled)
                throw new BusinessException("Нельзя изменить отмененную продажу");

            _mapper.Map(updateSaleDto, sale);
            await _saleRepository.UpdateAsync(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _saleRepository.GetWithDetailsAsync(id);
            if (sale == null)
                throw new BusinessException($"Продажа с ID {id} не найдена");

            if (sale.Status == SaleStatus.Completed)
                throw new BusinessException("Нельзя удалить завершенную продажу");

            await _saleRepository.DeleteAsync(sale);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByOrderIdAsync(int orderId)
        {
            var sales = await _saleRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByStockIdAsync(int stockId)
        {
            var sales = await _saleRepository.GetByStockIdAsync(stockId);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByStatusAsync(SaleStatus status)
        {
            var sales = await _saleRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _saleRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<SaleDto> CompleteSaleAsync(int id)
        {
            var sale = await _saleRepository.GetWithDetailsAsync(id);
            if (sale == null)
                throw new BusinessException($"Продажа с ID {id} не найдена");

            if (sale.Status != SaleStatus.Processing)
                throw new BusinessException("Можно завершить только продажу в обработке");

            sale.Status = SaleStatus.Completed;
            await _saleRepository.UpdateAsync(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<SaleDto> CancelSaleAsync(int id)
        {
            var sale = await _saleRepository.GetWithDetailsAsync(id);
            if (sale == null)
                throw new BusinessException($"Продажа с ID {id} не найдена");

            if (sale.Status != SaleStatus.Processing)
                throw new BusinessException("Можно отменить только продажу в обработке");

            sale.Status = SaleStatus.Cancelled;
            await _saleRepository.UpdateAsync(sale);
            return _mapper.Map<SaleDto>(sale);
        }
    }
}