using Application.Model.Users;

namespace Application.Data.Repository
{
    public interface IClientsStore : IStore<Client>
    {
        // Здесь можно добавить специфичные для клиентов методы, если потребуется
    }
} 