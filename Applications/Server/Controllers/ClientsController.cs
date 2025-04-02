using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
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

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
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
                return BadRequest($"Ошибка при получении списка клиентов: {ex.Message}");
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
                return NotFound($"Клиент с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при получении клиента: {ex.Message}");
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
                return NotFound($"Клиент с телефоном {phone} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при получении клиента: {ex.Message}");
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
        public async Task<ActionResult<ClientDto>> GetClientByName([FromQuery] string firstName, [FromQuery] string lastName)
        {
            try
            {
                var client = await _clientService.GetClientByNameAsync(firstName, lastName);
                return Ok(client);
            }
            catch (ClientNotFoundException)
            {
                return NotFound($"Клиент {firstName} {lastName} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при получении клиента: {ex.Message}");
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
                return BadRequest($"Ошибка при проверке существования клиента: {ex.Message}");
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
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(createClientDto.FirstName))
            {
                return BadRequest("Имя клиента обязательно для заполнения");
            }

            if (string.IsNullOrEmpty(createClientDto.LastName))
            {
                return BadRequest("Фамилия клиента обязательна для заполнения");
            }

            if (string.IsNullOrEmpty(createClientDto.Phone))
            {
                return BadRequest("Телефон клиента обязателен для заполнения");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Не удалось определить пользователя");
                }

                var client = await _clientService.CreateClientAsync(createClientDto, userId);
                return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании клиента: {ex.Message}");
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
                return BadRequest(ModelState);
            }

            try
            {
                var client = await _clientService.UpdateClientAsync(id, updateClientDto);
                return Ok(client);
            }
            catch (ClientNotFoundException)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении клиента: {ex.Message}");
            }
        }

        /// <summary>
        ///     Удалить клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <returns>Результат операции</returns>
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
            catch (ClientNotFoundException)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении клиента: {ex.Message}");
            }
        }
    }
}