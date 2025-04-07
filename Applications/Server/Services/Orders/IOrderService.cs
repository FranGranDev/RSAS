using Application.DTOs;
using Application.Models;

namespace Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string userId);
        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderDto>> GetOrdersByStockIdAsync(int stockId);
        Task<IEnumerable<OrderDto>> GetOrdersByStateAsync(Order.States state);
        Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<OrderDto> ExecuteOrderAsync(int id);
        Task<OrderDto> CompleteOrderAsync(int id);
        Task<bool> IsOrderOwnerAsync(int orderId, string userId);

        // Методы для работы с доставкой
        Task<DeliveryDto> UpdateDeliveryAsync(int orderId, UpdateDeliveryDto updateDeliveryDto);
        Task<IEnumerable<DeliveryDto>> GetDeliveriesByStatusAsync(string status);
        Task<IEnumerable<DeliveryDto>> GetDeliveriesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}