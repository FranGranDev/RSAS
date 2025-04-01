using Application.DTOs;

namespace Application.Services.Employees
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string userId);
        Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto);
        Task DeleteEmployeeAsync(int id);
        Task<EmployeeDto> GetEmployeeByUserIdAsync(string userId);
    }
}