using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Application.Services.Repository;
using Application.Utils;
using AutoMapper;
using Server.Services.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Services.Sales;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IStockRepository stockRepository,
            IMapper mapper,
            ISaleService saleService,
            IDeliveryRepository deliveryRepository)
        {
            _orderRepository = orderRepository;
            _stockRepository = stockRepository;
            _saleService = saleService;
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
            // Создаем базовый заказ
            var order = _mapper.Map<Order>(createOrderDto);
            order.UserId = userId;
            order.OrderDate = SystemTime.Now;
            order.ChangeDate = DateTime.UtcNow;
            order.State = Order.States.New;
            order.CancellationReason = "";
            order.Products = new List<OrderProduct>();

            // Сначала сохраняем заказ, чтобы получить его ID
            order = await _orderRepository.AddAsync(order);

            // Теперь создаем продукты заказа
            foreach (var productDto in createOrderDto.Products)
            {
                var orderProduct = _mapper.Map<OrderProduct>(productDto);
                orderProduct.OrderId = order.Id;
                order.Products.Add(orderProduct);
            }

            // Обновляем заказ с продуктами
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> ExecuteOrderAsync(int id, int stockId)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            // Проверяем существование склада
            var stock = await _stockRepository.GetByIdAsync(stockId);
            if (stock == null)
            {
                throw new StockNotFoundException(stockId);
            }

            // Проверяем наличие всех товаров на складе
            foreach (var product in order.Products)
            {
                var stockProduct = await _stockRepository.GetStockProductAsync(stockId, product.ProductId);
                if (stockProduct == null || stockProduct.Quantity < product.Quantity)
                {
                    throw new InsufficientStockException(product.ProductId, product.Quantity, 0);
                }
            }

            // Списываем товары со склада
            foreach (var product in order.Products)
            {
                var stockProduct = await _stockRepository.GetStockProductAsync(stockId, product.ProductId);
                stockProduct.Quantity -= product.Quantity;
                await _stockRepository.UpdateStockProductAsync(stockProduct);
            }

            // Связываем заказ со складом
            order.StockId = stockId;
            order.State = Order.States.InProcess;
            order.ChangeDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            if (order.State == Order.States.Completed || order.State == Order.States.Cancelled)
            {
                throw new InvalidOrderStateException($"Невозможно отменить заказ в состоянии {order.State}");
            }

            order.State = Order.States.Cancelled;
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
            
            await _saleService.CreateFromOrderAsync(order.Id);
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

        public async Task<IEnumerable<DeliveryDto>> GetDeliveriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var deliveries = await _deliveryRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<DeliveryDto>>(deliveries);
        }

        public async Task<OrderWithStockInfoDto> GetOrderWithStockInfoAsync(int orderId, int? stockId = null)
        {
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new OrderNotFoundException(orderId);
            }

            var orderDto = _mapper.Map<OrderWithStockInfoDto>(order);

            // Если склад не указан, используем текущий склад заказа
            var targetStockId = stockId ?? order.StockId;
            if (targetStockId == null)
            {
                throw new ArgumentException("Не указан склад для проверки наличия товаров");
            }

            var stock = await _stockRepository.GetWithStockProductsAsync(targetStockId.Value);
            if (stock == null)
            {
                throw new StockNotFoundException(targetStockId.Value);
            }

            // Обновляем информацию о товарах с учетом наличия на складе
            orderDto.Products = order.Products.Select(op =>
            {
                var stockProduct = stock.StockProducts.FirstOrDefault(sp => sp.ProductId == op.ProductId);
                return new OrderProductWithStockInfoDto
                {
                    ProductId = op.ProductId,
                    Name = op.Product.Name,
                    Price = op.Product.Price,
                    QuantityInOrder = op.Quantity,
                    QuantityInStock = stockProduct?.Quantity ?? 0,
                    IsEnough = stockProduct?.Quantity >= op.Quantity
                };
            }).ToList();

            orderDto.StockId = targetStockId;
            orderDto.StockName = stock.Name;

            return orderDto;
        }

        public async Task<DeliveryDto> GetDeliveryAsync(int orderId)
        {
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new OrderNotFoundException(orderId);
            }

            return new DeliveryDto
            {
                Id = order.Id,
                City = order.Delivery.City,
                Street = order.Delivery.Street,
                House = order.Delivery.House,
                Flat = order.Delivery.Flat,
                PostalCode = order.Delivery.PostalCode,
                DeliveryDate = order.Delivery.DeliveryDate
            };
        }

        public async Task<OrderDto> HoldOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            if (order.State != Order.States.New)
            {
                throw new InvalidOrderStateException($"Невозможно отложить заказ в состоянии {order.State}");
            }

            order.State = Order.States.OnHold;
            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> ResumeOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            if (order.State != Order.States.OnHold)
            {
                throw new InvalidOrderStateException($"Невозможно возобновить заказ в состоянии {order.State}");
            }

            order.State = Order.States.New;
            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }
    }
}