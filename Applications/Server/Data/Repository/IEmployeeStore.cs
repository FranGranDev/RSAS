using Application.Model.Users;

namespace Application.Data.Repository
{
    public interface IEmployeeStore : IStore<Employee>
    {
        // Здесь можно добавить специфичные для сотрудников методы, если потребуется
    }
} 