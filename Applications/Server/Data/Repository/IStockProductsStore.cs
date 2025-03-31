using Application.Model.Stocks;

namespace Application.Data.Repository
{
    public interface IStockProductsStore : IStore<StockProduct>
    {
        // Здесь можно добавить специфичные для продуктов на складе методы, если потребуется
    }
} 