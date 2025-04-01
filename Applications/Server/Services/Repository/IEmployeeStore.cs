using Application.Areas.Identity.Data;

namespace Application.Services.Repository
{
    public interface IEmployeeStore : IStringKeyStore<Employee>
    {
        Task<Employee> GetByUserIdAsync(string userId);
    }
}