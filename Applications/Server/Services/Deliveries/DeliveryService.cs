using Application.DTOs;
using Application.Exceptions;
using Application.Model.Orders;
using AutoMapper;
using Server.Services.Repository;

namespace Server.Services.Deliveries
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IMapper _mapper;

        public DeliveryService(IDeliveryRepository deliveryRepository, IMapper mapper)
        {
            _deliveryRepository = deliveryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DeliveryDto>> GetAllDeliveriesAsync()
        {
            var deliveries = await _deliveryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DeliveryDto>>(deliveries);
        }

        public async Task<DeliveryDto> GetDeliveryByIdAsync(int id)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            if (delivery == null)
                throw new BusinessException($"Доставка с ID {id} не найдена");

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto> GetDeliveryByOrderIdAsync(int orderId)
        {
            var delivery = await _deliveryRepository.GetByOrderIdAsync(orderId);
            if (delivery == null)
                throw new BusinessException($"Доставка для заказа с ID {orderId} не найдена");

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto> CreateDeliveryAsync(CreateDeliveryDto createDeliveryDto)
        {
            if (await _deliveryRepository.ExistsByOrderIdAsync(createDeliveryDto.OrderId))
                throw new BusinessException($"Доставка для заказа с ID {createDeliveryDto.OrderId} уже существует");

            var delivery = _mapper.Map<Delivery>(createDeliveryDto);
            await _deliveryRepository.AddAsync(delivery);

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto> UpdateDeliveryAsync(int id, UpdateDeliveryDto updateDeliveryDto)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            if (delivery == null)
                throw new BusinessException($"Доставка с ID {id} не найдена");

            if (delivery.Order.State == Order.States.Cancelled)
                throw new BusinessException("Нельзя изменить доставку отмененного заказа");

            _mapper.Map(updateDeliveryDto, delivery);
            await _deliveryRepository.UpdateAsync(delivery);

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task DeleteDeliveryAsync(int id)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            if (delivery == null)
                throw new BusinessException($"Доставка с ID {id} не найдена");

            if (delivery.Order.State == Order.States.Completed)
                throw new BusinessException("Нельзя удалить доставку завершенного заказа");

            await _deliveryRepository.DeleteAsync(delivery);
        }

        public async Task<IEnumerable<DeliveryDto>> GetDeliveriesByStatusAsync(string status)
        {
            var deliveries = await _deliveryRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<DeliveryDto>>(deliveries);
        }

        public async Task<IEnumerable<DeliveryDto>> GetDeliveriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var deliveries = await _deliveryRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<DeliveryDto>>(deliveries);
        }
    }
} 