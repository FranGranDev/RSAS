using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using AutoMapper;
using Server.Services.Repository;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(string userId)
        {
            var employee = await _employeeRepository.GetWithUserAsync(userId);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с ID {userId} не найден");
            }

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string userId)
        {
            // Проверяем, не существует ли уже сотрудник с таким UserId
            var existingEmployee = await _employeeRepository.GetWithUserAsync(userId);
            if (existingEmployee != null)
            {
                throw new BusinessException("Сотрудник с таким UserId уже существует");
            }

            // Проверяем, не существует ли уже сотрудник с таким телефоном
            if (await _employeeRepository.ExistsByPhoneAsync(createEmployeeDto.Phone))
            {
                throw new BusinessException("Сотрудник с таким номером телефона уже существует");
            }

            var employee = _mapper.Map<Employee>(createEmployeeDto);
            employee.UserId = userId;
            await _employeeRepository.AddAsync(employee);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(string userId, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await _employeeRepository.GetWithUserAsync(userId);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с ID {userId} не найден");
            }

            // Проверяем, не занят ли новый номер телефона другим сотрудником
            if (employee.Phone != updateEmployeeDto.Phone &&
                await _employeeRepository.ExistsByPhoneAsync(updateEmployeeDto.Phone))
            {
                throw new BusinessException("Сотрудник с таким номером телефона уже существует");
            }

            _mapper.Map(updateEmployeeDto, employee);
            await _employeeRepository.UpdateAsync(employee);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task DeleteEmployeeAsync(string userId)
        {
            var employee = await _employeeRepository.GetWithUserAsync(userId);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с ID {userId} не найден");
            }

            await _employeeRepository.DeleteAsync(userId);
        }

        public async Task<EmployeeDto> GetEmployeeByPhoneAsync(string phone)
        {
            var employee = await _employeeRepository.GetByPhoneAsync(phone);
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с телефоном {phone} не найден");
            }

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> GetEmployeeByNameAsync(string firstName, string lastName)
        {
            var employees = await _employeeRepository.GetByNameAsync(firstName, lastName);
            var employee = employees.FirstOrDefault();
            if (employee == null)
            {
                throw new BusinessException($"Сотрудник с именем {firstName} {lastName} не найден");
            }

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByRoleAsync(string role)
        {
            var employees = await _employeeRepository.GetByRoleAsync(role);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _employeeRepository.ExistsByPhoneAsync(phone);
        }
    }
}