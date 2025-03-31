using Application.Model.Products;

namespace Application.Data.Repository
{
    public interface IProductsStore : IStore<Product>
    {
        // Здесь можно добавить специфичные для продуктов методы, если потребуется
    }
} 