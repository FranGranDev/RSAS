using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Data.Repository;
using Application.DTOs;
using Application.Areas.Identity.Data;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeStore _employeeStore;

        public EmployeesController(IEmployeeStore employeeStore)
        {
            _employeeStore = employeeStore;
        }

        // GET: api/Employees
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _employeeStore.GetAllAsync();
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

        // GET: api/Employees/5
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _employeeStore.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Role = employee.Role,
                Email = employee.Email,
                Phone = employee.Phone
            };
        }

        // POST: api/Employees
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createEmployeeDto)
        {
            var employee = new Employee
            {
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                Role = createEmployeeDto.Role,
                Email = createEmployeeDto.Email,
                Phone = createEmployeeDto.Phone
            };

            await _employeeStore.Save(employee);

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employee.Id },
                new EmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Role = employee.Role,
                    Email = employee.Email,
                    Phone = employee.Phone
                });
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await _employeeStore.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.FirstName = updateEmployeeDto.FirstName;
            employee.LastName = updateEmployeeDto.LastName;
            employee.Role = updateEmployeeDto.Role;
            employee.Email = updateEmployeeDto.Email;
            employee.Phone = updateEmployeeDto.Phone;

            await _employeeStore.Save(employee);

            return NoContent();
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeeStore.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await _employeeStore.Delete(id);

            return NoContent();
        }
    }
} 