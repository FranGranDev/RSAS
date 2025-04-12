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
        ///     Проверить существование клиента для текущего пользователя
        /// </summary>
        /// <returns>Результат проверки</returns>
        [HttpGet("exists")]
        [Authorize]
        public async Task<ActionResult<bool>> ExistsForCurrentUser()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var exists = await _clientService.ExistsByUserIdAsync(userId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при проверке существования клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Создать нового клиента для текущего пользователя
        /// </summary>
        /// <param name="createClientDto">Данные для создания клиента</param>
        /// <returns>Созданный клиент</returns>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPost("create-for-current")]
        [Authorize]
        public async Task<ActionResult<ClientDto>> CreateForCurrentUser(CreateClientDto createClientDto)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var client = await _clientService.CreateClientAsync(createClientDto, userId);
                return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при создании клиента: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Создать полного пользователя (AppUser + Client)
        /// </summary>
        /// <param name="createFullUserDto">Данные для создания пользователя</param>
        /// <returns>Созданный пользователь и стандартный пароль</returns>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPost("create-full")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<CreateFullUserResponseDto>> CreateFullUser(CreateFullUserDto createFullUserDto)
        {
            try
            {
                // Создаем нового пользователя
                var user = new AppUser
                {
                    UserName = createFullUserDto.Email,
                    Email = createFullUserDto.Email,
                    EmailConfirmed = true,
                    PhoneNumber = createFullUserDto.Phone,
                    PhoneNumberConfirmed = true
                };

                var defaultPassword = AppConst.Password.DefaultPassword;
                var result = await _userManager.CreateAsync(user, defaultPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }

                // Добавляем роль клиента
                await _userManager.AddToRoleAsync(user, AppConst.Roles.Client);

                // Создаем клиента
                var createClientDto = new CreateClientDto
                {
                    FirstName = createFullUserDto.FirstName,
                    LastName = createFullUserDto.LastName,
                    Phone = createFullUserDto.Phone
                };

                var client = await _clientService.CreateClientAsync(createClientDto, user.Id);

                return Ok(new CreateFullUserResponseDto
                {
                    Email = user.Email,
                    DefaultPassword = defaultPassword,
                    Client = client
                });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при создании пользователя: {ex.Message}" });
            }
        }

        /// <summary>
        ///     Обновить данные текущего пользователя
        /// </summary>
        /// <param name="updateClientDto">Данные для обновления</param>
        /// <returns>Обновленный клиент</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Клиент не найден</response>
        [HttpPut("update-self")]
        [Authorize]
        public async Task<ActionResult<ClientDto>> UpdateSelf(UpdateClientDto updateClientDto)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var client = await _clientService.UpdateClientAsync(userId, updateClientDto);
                return Ok(client);
            }
            catch (ClientNotFoundException)
            {
                return NotFound(new { error = "Клиент не найден" });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
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
                await _clientService.DeleteClientAsync(id);
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

        /// <summary>
        ///     Получить данные текущего клиента
        /// </summary>
        /// <returns>Данные клиента</returns>
        /// <response code="404">Клиент не найден</response>
        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<ClientDto>> GetCurrentClient()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var client = await _clientService.GetClientByIdAsync(userId);
                return Ok(client);
            }
            catch (ClientNotFoundException)
            {
                return NotFound(new { error = "Клиент не найден" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Ошибка при получении данных клиента: {ex.Message}" });
            }
        }
    }
}