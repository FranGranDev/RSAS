using Application.Model.Stocks;

namespace Application.Services.Repository
{
    public interface IStockStore : IStore<Stock>
    {
        // Здесь можно добавить специфичные для складов методы, если потребуется
    }
}