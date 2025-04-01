using Application.Areas.Identity.Data;

namespace Server.Services.Repository;

public interface IClientRepository : IRepository<Client, string>
{
    // Получение клиента по номеру телефона
    Task<Client?> GetByPhoneAsync(string phone);
    
    // Получение клиента по имени и фамилии
    Task<IEnumerable<Client>> GetByNameAsync(string firstName, string lastName);
    
    // Получение клиента с включением связанного пользователя
    Task<Client?> GetWithUserAsync(string userId);
    
    // Проверка существования клиента по телефону
    Task<bool> ExistsByPhoneAsync(string phone);
} 