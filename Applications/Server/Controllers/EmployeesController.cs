using System.Security.Claims;
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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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
        /// <response code="403">Недостаточно прав для просмотра всех сотрудников</response>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        /// <summary>
        ///     Получить сотрудника по ID
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <returns>Информация о сотруднике</returns>
        /// <response code="403">Недостаточно прав для просмотра сотрудника</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(string id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(employee);
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound($"Сотрудник с ID {id} не найден");
            }
        }

        /// <summary>
        ///     Создать нового сотрудника
        /// </summary>
        /// <param name="createEmployeeDto">Данные для создания сотрудника</param>
        /// <returns>Созданный сотрудник</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для создания сотрудника</response>
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var employee = await _employeeService.CreateEmployeeAsync(createEmployeeDto, userId);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании сотрудника: {ex.Message}");
            }
        }

        /// <summary>
        ///     Обновить информацию о сотруднике
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <param name="updateEmployeeDto">Данные для обновления сотрудника</param>
        /// <returns>Обновленный сотрудник</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для обновления сотрудника</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(string id, UpdateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var employee = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);
                return Ok(employee);
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound($"Сотрудник с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении сотрудника: {ex.Message}");
            }
        }

        /// <summary>
        ///     Удалить сотрудника
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <returns>Результат операции</returns>
        /// <response code="403">Недостаточно прав для удаления сотрудника</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                return NoContent();
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound($"Сотрудник с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении сотрудника: {ex.Message}");
            }
        }

        /// <summary>
        ///     Получить сотрудников по роли
        /// </summary>
        /// <param name="role">Роль сотрудника</param>
        /// <returns>Список сотрудников</returns>
        /// <response code="403">Недостаточно прав для просмотра сотрудников</response>
        [HttpGet("role/{role}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByRole(string role)
        {
            var employees = await _employeeService.GetEmployeesByRoleAsync(role);
            return Ok(employees);
        }

        /// <summary>
        ///     Получить сотрудника по телефону
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>Информация о сотруднике</returns>
        /// <response code="403">Недостаточно прав для просмотра сотрудника</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpGet("phone/{phone}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByPhone(string phone)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByPhoneAsync(phone);
                return Ok(employee);
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound($"Сотрудник с телефоном {phone} не найден");
            }
        }

        /// <summary>
        ///     Получить сотрудника по имени и фамилии
        /// </summary>
        /// <param name="firstName">Имя</param>
        /// <param name="lastName">Фамилия</param>
        /// <returns>Информация о сотруднике</returns>
        /// <response code="403">Недостаточно прав для просмотра сотрудника</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpGet("name")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByName(
            [FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByNameAsync(firstName, lastName);
                return Ok(employee);
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound($"Сотрудник с именем {firstName} {lastName} не найден");
            }
        }

        /// <summary>
        ///     Проверить существование сотрудника по телефону
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>Результат проверки</returns>
        /// <response code="403">Недостаточно прав для проверки</response>
        [HttpGet("exists/phone/{phone}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<bool>> ExistsByPhone(string phone)
        {
            var exists = await _employeeService.ExistsByPhoneAsync(phone);
            return Ok(exists);
        }
    }
}