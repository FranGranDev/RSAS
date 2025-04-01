using Application.DTOs;

namespace Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByClientIdAsync(int clientId);
        Task<OrderDto> ExecuteOrderAsync(int id);
        Task<OrderDto> CompleteOrderAsync(int id);
    }
}