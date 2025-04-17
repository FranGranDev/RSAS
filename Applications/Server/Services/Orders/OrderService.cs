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
                throw new OrderNotFoundException(id);
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string userId)
        {
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

        public async Task<OrderDto> ExecuteOrderAsync(int id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            if (!order.StockId.HasValue)
            {
                throw new StockNotFoundException(id);
            }

            // Проверяем наличие товаров на складе
            foreach (var product in order.Products)
            {
                var stockProduct = await _stockRepository.GetStockProductAsync(order.StockId.Value, product.ProductId);
                if (stockProduct == null)
                {
                    throw new InsufficientStockException(product.ProductId, product.Quantity, 0);
                }

                if (stockProduct.Quantity < product.Quantity)
                {
                    throw new InsufficientStockException(product.ProductId, product.Quantity, stockProduct.Quantity);
                }
            }

            // Списываем товары со склада
            foreach (var product in order.Products)
            {
                var stockProduct = await _stockRepository.GetStockProductAsync(order.StockId.Value, product.ProductId);
                stockProduct.Quantity -= product.Quantity;
                await _stockRepository.UpdateStockProductAsync(stockProduct);
            }

            order.State = Order.States.InProcess;
            order.ChangeDate = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            if (order.State == Order.States.Completed)
            {
                throw new InvalidOrderStateException("Нельзя отменить завершенный заказ");
            }

            // Возвращаем товары на склад, если заказ был в процессе выполнения
            if (order.State == Order.States.InProcess && order.StockId.HasValue)
            {
                foreach (var product in order.Products)
                {
                    var stockProduct = await _stockRepository.GetStockProductAsync(order.StockId.Value, product.ProductId);
                    if (stockProduct != null)
                    {
                        stockProduct.Quantity += product.Quantity;
                        await _stockRepository.UpdateStockProductAsync(stockProduct);
                    }
                }
            }

            order.State = Order.States.Cancelled;
            order.ChangeDate = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            if (order.State == Order.States.Completed)
            {
                throw new InvalidOrderStateException("Нельзя изменить завершенный заказ");
            }

            if (order.State == Order.States.Cancelled)
            {
                throw new InvalidOrderStateException("Нельзя изменить отмененный заказ");
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
                throw new OrderNotFoundException(id);
            }

            if (order.State == Order.States.Completed)
            {
                throw new InvalidOrderStateException("Нельзя удалить завершенный заказ");
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

        public async Task<OrderDto> CompleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            if (order.State != Order.States.InProcess)
            {
                throw new InvalidOrderStateException("Можно завершить только заказ в процессе");
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

            return order.UserId == userId;
        }

        public async Task<DeliveryDto> UpdateDeliveryAsync(int orderId, UpdateDeliveryDto updateDeliveryDto)
        {
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new OrderNotFoundException(orderId);
            }

            if (order.State == Order.States.Cancelled)
            {
                throw new InvalidOrderStateException("Нельзя изменить доставку отмененного заказа");
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