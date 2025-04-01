using Application.DTOs;
using Application.Model.Orders;

namespace Server.Services.Orders
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
    }
}