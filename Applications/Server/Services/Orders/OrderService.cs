using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Application.Services.Repository;
using AutoMapper;
using Server.Services.Repository;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IStockRepository _stockRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IStockRepository stockRepository,
            IMapper mapper,
            IDeliveryRepository deliveryRepository)
        {
            _orderRepository = orderRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
            _deliveryRepository = deliveryRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string userId)
        {
            if (!createOrderDto.StockId.HasValue)
            {
                throw new BusinessException("Не указан склад");
            }

            // Проверяем наличие склада
            var stock = await _stockRepository.GetByIdAsync(createOrderDto.StockId.Value);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {createOrderDto.StockId} не найден");
            }

            // Проверяем наличие товаров на складе
            foreach (var product in createOrderDto.Products)
            {
                var stockProduct = await _stockRepository.GetWithStockProductsAsync(createOrderDto.StockId.Value);
                var productInStock = stockProduct.StockProducts.FirstOrDefault(sp => sp.ProductId == product.ProductId);

                if (productInStock == null)
                {
                    throw new BusinessException($"Товар с ID {product.ProductId} отсутствует на складе");
                }

                if (productInStock.Quantity < product.Quantity)
                {
                    throw new BusinessException($"Недостаточно товара с ID {product.ProductId} на складе");
                }
            }

            var order = _mapper.Map<Order>(createOrderDto);
            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            order.ChangeDate = DateTime.UtcNow;
            order.State = Order.States.New;

            await _orderRepository.AddAsync(order);

            // Создаем доставку
            if (createOrderDto.Delivery != null)
            {
                var delivery = _mapper.Map<Delivery>(createOrderDto.Delivery);
                delivery.OrderId = order.Id;
                delivery.Status = "Создана";
                await _deliveryRepository.AddAsync(delivery);
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State == Order.States.Completed)
            {
                throw new BusinessException("Нельзя изменить завершенный заказ");
            }

            if (order.State == Order.States.Cancelled)
            {
                throw new BusinessException("Нельзя изменить отмененный заказ");
            }

            _mapper.Map(updateOrderDto, order);
            order.ChangeDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State == Order.States.Completed)
            {
                throw new BusinessException("Нельзя удалить завершенный заказ");
            }

            await _orderRepository.DeleteAsync(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStockIdAsync(int stockId)
        {
            var orders = await _orderRepository.GetByStockIdAsync(stockId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStateAsync(Order.States state)
        {
            var orders = await _orderRepository.GetByStateAsync(state);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> ExecuteOrderAsync(int id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State != Order.States.New)
            {
                throw new BusinessException("Можно выполнить только новый заказ");
            }

            order.State = Order.States.InProcess;
            order.ChangeDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CompleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State != Order.States.InProcess)
            {
                throw new BusinessException("Можно завершить только заказ в процессе");
            }

            order.State = Order.States.Completed;
            order.ChangeDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> IsOrderOwnerAsync(int orderId, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            // Проверяем, является ли пользователь создателем заказа
            return order.UserId == userId;
        }

        // Методы для работы с доставкой
        public async Task<DeliveryDto> UpdateDeliveryAsync(int orderId, UpdateDeliveryDto updateDeliveryDto)
        {
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {orderId} не найден");
            }

            if (order.State == Order.States.Cancelled)
            {
                throw new BusinessException("Нельзя изменить доставку отмененного заказа");
            }

            var delivery = await _deliveryRepository.GetByOrderIdAsync(orderId);
            if (delivery == null)
            {
                throw new BusinessException($"Доставка для заказа с ID {orderId} не найдена");
            }

            _mapper.Map(updateDeliveryDto, delivery);
            await _deliveryRepository.UpdateAsync(delivery);

            return _mapper.Map<DeliveryDto>(delivery);
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