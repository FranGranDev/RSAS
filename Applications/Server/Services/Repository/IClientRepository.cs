using Application.Models;

namespace Server.Services.Repository
{
    public interface IClientRepository : IRepository<Client, string>
    {
        // Получение всех клиентов с включением связанного пользователя
        Task<IEnumerable<Client>> GetAllWithUserAsync();

        // Получение клиента по номеру телефона
        Task<Client?> GetByPhoneAsync(string phone);

        // Получение клиента по номеру телефона с включением связанного пользователя
        Task<Client?> GetByPhoneWithUserAsync(string phone);

        // Получение клиента по имени и фамилии
        Task<IEnumerable<Client>> GetByNameAsync(string firstName, string lastName);

        // Получение клиента по имени и фамилии с включением связанного пользователя
        Task<IEnumerable<Client>> GetByNameWithUserAsync(string firstName, string lastName);

        // Получение клиента с включением связанного пользователя
        Task<Client?> GetWithUserAsync(string userId);

        // Проверка существования клиента по телефону
        Task<bool> ExistsByPhoneAsync(string phone);
    }
}