using Application.DTOs;
using Application.Model.Orders;

namespace Server.Services.Deliveries
{
    public interface IDeliveryService
    {
        Task<IEnumerable<DeliveryDto>> GetAllDeliveriesAsync();
        Task<DeliveryDto> GetDeliveryByIdAsync(int id);
        Task<DeliveryDto> GetDeliveryByOrderIdAsync(int orderId);
        Task<DeliveryDto> CreateDeliveryAsync(CreateDeliveryDto createDeliveryDto);
        Task<DeliveryDto> UpdateDeliveryAsync(int id, UpdateDeliveryDto updateDeliveryDto);
        Task DeleteDeliveryAsync(int id);
        Task<IEnumerable<DeliveryDto>> GetDeliveriesByStatusAsync(string status);
        Task<IEnumerable<DeliveryDto>> GetDeliveriesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
} 