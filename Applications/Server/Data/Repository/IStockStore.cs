using Application.Model.Stocks;

namespace Application.Data.Repository
{
    public interface IStockStore : IStore<Stock>
    {
        // Здесь можно добавить специфичные для складов методы, если потребуется
    }
} 