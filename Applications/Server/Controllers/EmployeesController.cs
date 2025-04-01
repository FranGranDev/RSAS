using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    /// <summary>
    ///     Контроллер для управления сотрудниками
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        ///     Получить список всех сотрудников
        /// </summary>
        /// <returns>Список сотрудников</returns>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Role = e.Role,
                Email = e.Email,
                Phone = e.Phone
            }));
        }

        /// <summary>
        ///     Получить сотрудника по ID
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <returns>Информация о сотруднике</returns>
        /// <response code="404">Сотрудник не найден</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(employee);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Сотрудник с ID {id} не найден", ex);
            }
        }

        /// <summary>
        ///     Создать нового сотрудника
        /// </summary>
        /// <param name="createEmployeeDto">Данные для создания сотрудника</param>
        /// <returns>Созданный сотрудник</returns>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные сотрудника", ModelState);
            }

            var employee = await _employeeService.CreateEmployeeAsync(createEmployeeDto);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        /// <summary>
        ///     Обновить существующего сотрудника
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <param name="updateEmployeeDto">Данные для обновления сотрудника</param>
        /// <returns>Обновленный сотрудник</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные сотрудника", ModelState);
            }

            try
            {
                var employee = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);
                return Ok(employee);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Сотрудник с ID {id} не найден", ex);
            }
        }

        /// <summary>
        ///     Удалить сотрудника
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <returns>Результат операции</returns>
        /// <response code="404">Сотрудник не найден</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Сотрудник с ID {id} не найден", ex);
            }
        }
    }
}