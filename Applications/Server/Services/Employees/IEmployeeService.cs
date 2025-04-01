using Application.DTOs;

namespace Application.Services.Employees
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(string userId);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string userId);
        Task<EmployeeDto> UpdateEmployeeAsync(string userId, UpdateEmployeeDto updateEmployeeDto);
        Task DeleteEmployeeAsync(string userId);
        Task<EmployeeDto> GetEmployeeByPhoneAsync(string phone);
        Task<EmployeeDto> GetEmployeeByNameAsync(string firstName, string lastName);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByRoleAsync(string role);
        Task<bool> ExistsByPhoneAsync(string phone);
    }
}