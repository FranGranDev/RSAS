using Application.Model.Orders;

namespace Application.Data.Repository
{
    public interface IOrderStore : IStore<Order>
    {
        // Здесь можно добавить специфичные для заказов методы, если потребуется
    }
} 