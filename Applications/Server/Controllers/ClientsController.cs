using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Data.Repository;
using Application.DTOs;
using Application.Areas.Identity.Data;
using Application.Exceptions;
using Application.Services;
using Application.Services.Clients;

namespace Application.Controllers
{
    /// <summary>
    /// Контроллер для управления клиентами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Получить список всех клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients.Select(c => new ClientDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone
            }));
        }

        /// <summary>
        /// Получить клиента по ID
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <returns>Информация о клиенте</returns>
        /// <response code="404">Клиент не найден</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                return Ok(client);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Клиент с ID {id} не найден", ex);
            }
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="createClientDto">Данные для создания клиента</param>
        /// <returns>Созданный клиент</returns>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto createClientDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные клиента", ModelState);
            }

            var client = await _clientService.CreateClientAsync(createClientDto);
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        /// <summary>
        /// Обновить существующего клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="updateClientDto">Данные для обновления клиента</param>
        /// <returns>Обновленный клиент</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Клиент не найден</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<ClientDto>> UpdateClient(int id, UpdateClientDto updateClientDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные клиента", ModelState);
            }

            try
            {
                var client = await _clientService.UpdateClientAsync(id, updateClientDto);
                return Ok(client);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Клиент с ID {id} не найден", ex);
            }
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <returns>Результат операции</returns>
        /// <response code="404">Клиент не найден</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Клиент с ID {id} не найден", ex);
            }
        }
    }
} 