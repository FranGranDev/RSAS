using Application.Model.Stocks;

namespace Application.Services.Repository
{
    public interface IProductsStore : IStore<Product>
    {
        // Здесь можно добавить специфичные для продуктов методы, если потребуется
    }
}