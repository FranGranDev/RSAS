using Application.Data;
using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    /// <summary>
    ///     Контроллер для управления клиентами
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly UserManager<AppUser> _userManager;

        public ClientsController(IClientService clientService, UserManager<AppUser> userManager)
        {
            _clientService = clientService;
            _userManager = userManager;
        }

        /// <summary>
        ///     Получить список всех клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
        /// <response code="403">Недостаточно прав для просмотра всех клиентов</response>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            try
            {
                var clients = await _clientService.GetAllClientsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при получении списка клиентов: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Получить клиента по ID
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <returns>Информация о клиенте</returns>
        /// <response code="404">Клиент не найден</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ClientDto>> GetClient(string id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                return Ok(client);
            }
            catch (ClientNotFoundException)
            {
                return NotFound(new { error = $"Клиент с ID {id} не найден" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при получении клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Получить клиента по телефону
        /// </summary>
        /// <param name="phone">Номер телефона клиента</param>
        /// <returns>Информация о клиенте</returns>
        /// <response code="404">Клиент не найден</response>
        [HttpGet("by-phone/{phone}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ClientDto>> GetClientByPhone(string phone)
        {
            try
            {
                var client = await _clientService.GetClientByPhoneAsync(phone);
                return Ok(client);
            }
            catch (ClientNotFoundException)
            {
                return NotFound(new { error = $"Клиент с телефоном {phone} не найден" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при получении клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Получить клиента по имени и фамилии
        /// </summary>
        /// <param name="firstName">Имя клиента</param>
        /// <param name="lastName">Фамилия клиента</param>
        /// <returns>Информация о клиенте</returns>
        /// <response code="404">Клиент не найден</response>
        [HttpGet("by-name")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ClientDto>> GetClientByName([FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            try
            {
                var client = await _clientService.GetClientByNameAsync(firstName, lastName);
                return Ok(client);
            }
            catch (ClientNotFoundException)
            {
                return NotFound(new { error = $"Клиент {firstName} {lastName} не найден" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при получении клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Проверить существование клиента по телефону
        /// </summary>
        /// <param name="phone">Номер телефона клиента</param>
        /// <returns>Результат проверки</returns>
        [HttpGet("exists/{phone}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<bool>> ExistsByPhone(string phone)
        {
            try
            {
                var exists = await _clientService.ExistsByPhoneAsync(phone);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при проверке существования клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Создать нового клиента
        /// </summary>
        /// <param name="createClientDto">Данные для создания клиента</param>
        /// <returns>Созданный клиент</returns>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPost]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto createClientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                    { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            if (string.IsNullOrEmpty(createClientDto.FirstName))
            {
                return BadRequest(new { error = "Имя клиента обязательно для заполнения" });
            }

            if (string.IsNullOrEmpty(createClientDto.LastName))
            {
                return BadRequest(new { error = "Фамилия клиента обязательна для заполнения" });
            }

            if (string.IsNullOrEmpty(createClientDto.Phone))
            {
                return BadRequest(new { error = "Телефон клиента обязателен для заполнения" });
            }

            if (string.IsNullOrEmpty(createClientDto.Email))
            {
                return BadRequest(new { error = "Email клиента обязателен для заполнения" });
            }

            try
            {
                // Создаем нового пользователя
                var user = new AppUser
                {
                    UserName = createClientDto.Email,
                    Email = createClientDto.Email,
                    EmailConfirmed = true,
                    PhoneNumber = createClientDto.Phone,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }

                // Добавляем роль клиента
                await _userManager.AddToRoleAsync(user, AppConst.Roles.Client);

                // Создаем клиента
                var client = await _clientService.CreateClientAsync(createClientDto, user.Id);
                return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при создании клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Обновить существующего клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="updateClientDto">Данные для обновления клиента</param>
        /// <returns>Обновленный клиент</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Клиент не найден</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ClientDto>> UpdateClient(string id, UpdateClientDto updateClientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                    { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                // Получаем текущего клиента
                var currentClient = await _clientService.GetClientByIdAsync(id);
                if (currentClient == null)
                {
                    return NotFound(new { error = $"Клиент с ID {id} не найден" });
                }

                // Получаем пользователя
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { error = $"Пользователь с ID {id} не найден" });
                }

                // Обновляем email пользователя, если он изменился
                if (user.Email != updateClientDto.Email)
                {
                    user.Email = updateClientDto.Email;
                    user.UserName = updateClientDto.Email;
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                    }
                }

                // Обновляем клиента
                var updatedClient = await _clientService.UpdateClientAsync(id, updateClientDto);
                return Ok(updatedClient);
            }
            catch (ClientNotFoundException)
            {
                return NotFound(new { error = $"Клиент с ID {id} не найден" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при обновлении клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Удалить клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <returns>Результат операции</returns>
        /// <response code="403">Недостаточно прав для удаления клиента</response>
        /// <response code="404">Клиент не найден</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> DeleteClient(string id)
        {
            try
            {
                // Получаем клиента
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    return NotFound(new { error = $"Клиент с ID {id} не найден" });
                }

                // Получаем пользователя
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { error = $"Пользователь с ID {id} не найден" });
                }

                // Удаляем клиента
                await _clientService.DeleteClientAsync(id);

                // Удаляем пользователя
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }

                return NoContent();
            }
            catch (BusinessException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при удалении клиента: {ex.Message}" });
            }
        }
    }
}