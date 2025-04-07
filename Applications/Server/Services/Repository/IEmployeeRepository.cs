using Application.Models;

namespace Server.Services.Repository
{
    public interface IEmployeeRepository : IRepository<Employee, string>
    {
        // Получение сотрудника по номеру телефона
        Task<Employee?> GetByPhoneAsync(string phone);

        // Получение сотрудника по имени и фамилии
        Task<IEnumerable<Employee>> GetByNameAsync(string firstName, string lastName);

        // Получение сотрудника с включением связанного пользователя
        Task<Employee?> GetWithUserAsync(string userId);

        // Получение сотрудников по роли
        Task<IEnumerable<Employee>> GetByRoleAsync(string role);

        // Проверка существования сотрудника по телефону
        Task<bool> ExistsByPhoneAsync(string phone);

        // Получение всех сотрудников с включением связанных пользователей
        Task<IEnumerable<Employee>> GetAllWithUsersAsync();
    }
}