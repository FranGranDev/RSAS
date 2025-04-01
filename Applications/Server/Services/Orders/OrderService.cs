using Application.DTOs;
using Application.Exceptions;
using Application.Model.Orders;
using Application.Services.Repository;
using AutoMapper;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderStore _orderStore;
        private readonly IStockProductsStore _stockProductsStore;
        private readonly IStockStore _stockStore;

        public OrderService(
            IOrderStore orderStore,
            IStockStore stockStore,
            IStockProductsStore stockProductsStore,
            IMapper mapper)
        {
            _orderStore = orderStore;
            _stockStore = stockStore;
            _stockProductsStore = stockProductsStore;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderStore.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderStore.GetByIdAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            if (!createOrderDto.StockId.HasValue)
            {
                throw new BusinessException("Не указан склад");
            }

            // Проверяем наличие склада
            var stock = await _stockStore.GetByIdAsync(createOrderDto.StockId.Value);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {createOrderDto.StockId} не найден");
            }

            // Проверяем наличие товаров на складе
            foreach (var product in createOrderDto.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    createOrderDto.StockId.Value, product.ProductId);

                if (stockProduct == null || stockProduct.Quantity < product.Quantity)
                {
                    throw new BusinessException($"Недостаточно товара {product.ProductId} на складе");
                }
            }

            var order = _mapper.Map<Order>(createOrderDto);
            order.OrderDate = DateTime.UtcNow;
            order.ChangeDate = DateTime.UtcNow;
            order.State = Order.States.New;

            await _orderStore.SaveAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderStore.GetByIdAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State != Order.States.New)
            {
                throw new BusinessException("Можно обновлять только новые заказы");
            }

            _mapper.Map(updateOrderDto, order);
            order.ChangeDate = DateTime.UtcNow;

            await _orderStore.SaveAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderStore.GetByIdAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State != Order.States.New)
            {
                throw new BusinessException("Можно удалять только новые заказы");
            }

            await _orderStore.DeleteAsync(id);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByClientIdAsync(int clientId)
        {
            var orders = await _orderStore.GetByClientIdAsync(clientId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> ExecuteOrderAsync(int id)
        {
            var order = await _orderStore.GetByIdAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State != Order.States.New)
            {
                throw new BusinessException("Можно выполнять только новые заказы");
            }

            if (!order.StockId.HasValue)
            {
                throw new BusinessException("Не указан склад");
            }

            var stock = await _stockStore.GetByIdAsync(order.StockId.Value);
            if (stock == null)
            {
                throw new BusinessException($"Склад с ID {order.StockId} не найден");
            }

            // Проверяем наличие товаров на складе
            foreach (var product in order.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    order.StockId.Value, product.ProductId);

                if (stockProduct == null || stockProduct.Quantity < product.Quantity)
                {
                    throw new BusinessException($"Недостаточно товара {product.ProductId} на складе");
                }
            }

            // Уменьшаем количество товаров на складе
            foreach (var product in order.Products)
            {
                var stockProduct = await _stockProductsStore.GetByStockAndProductIdAsync(
                    order.StockId.Value, product.ProductId);
                stockProduct.Quantity -= product.Quantity;
            }

            order.State = Order.States.InProcess;
            order.ChangeDate = DateTime.UtcNow;

            await _orderStore.SaveAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CompleteOrderAsync(int id)
        {
            var order = await _orderStore.GetByIdAsync(id);
            if (order == null)
            {
                throw new BusinessException($"Заказ с ID {id} не найден");
            }

            if (order.State != Order.States.InProcess)
            {
                throw new BusinessException("Можно завершать только заказы в процессе");
            }

            order.State = Order.States.Completed;
            order.ChangeDate = DateTime.UtcNow;

            await _orderStore.SaveAsync(order);
            return _mapper.Map<OrderDto>(order);
        }
    }
}