using Application.Areas.Identity.Data;
using Application.DTOs;
using Application.Exceptions;
using Application.Services.Repository;
using AutoMapper;

namespace Application.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeStore _employeeStore;
        private readonly IMapper _mapper;

        public EmployeeService(
            IEmployeeStore employeeStore,
            IMapper mapper)
        {
            _employeeStore = employeeStore;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeStore.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeStore.GetByIdAsync(id);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с ID {id} не найден");
            }

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string userId)
        {
            // Проверяем, не существует ли уже сотрудник с таким UserId
            var existingEmployee = await _employeeStore.GetByUserIdAsync(userId);
            if (existingEmployee != null)
            {
                throw new BusinessException("Сотрудник с таким UserId уже существует");
            }

            var employee = _mapper.Map<Employee>(createEmployeeDto);
            employee.UserId = userId;
            await _employeeStore.SaveAsync(employee);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await _employeeStore.GetByIdAsync(id);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с ID {id} не найден");
            }

            _mapper.Map(updateEmployeeDto, employee);
            await _employeeStore.SaveAsync(employee);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeStore.GetByIdAsync(id);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с ID {id} не найден");
            }

            await _employeeStore.DeleteAsync(id);
        }

        public async Task<EmployeeDto> GetEmployeeByUserIdAsync(string userId)
        {
            var employee = await _employeeStore.GetByUserIdAsync(userId);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с UserId {userId} не найден");
            }

            return _mapper.Map<EmployeeDto>(employee);
        }
    }
}