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
    public class ClientsController : ControllerBase
    {
        private readonly IClientsStore _clientsStore;

        public ClientsController(IClientsStore clientsStore)
        {
            _clientsStore = clientsStore;
        }

        // GET: api/Clients
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _clientsStore.GetAllAsync();
            return Ok(clients.Select(c => new ClientDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone
            }));
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            var client = await _clientsStore.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = client.Phone
            };
        }

        // POST: api/Clients
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto createClientDto)
        {
            var client = new Client
            {
                FirstName = createClientDto.FirstName,
                LastName = createClientDto.LastName,
                Email = createClientDto.Email,
                Phone = createClientDto.Phone
            };

            await _clientsStore.Save(client);

            return CreatedAtAction(
                nameof(GetClient),
                new { id = client.Id },
                new ClientDto
                {
                    Id = client.Id,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    Phone = client.Phone
                });
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateClient(int id, UpdateClientDto updateClientDto)
        {
            var client = await _clientsStore.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            client.FirstName = updateClientDto.FirstName;
            client.LastName = updateClientDto.LastName;
            client.Email = updateClientDto.Email;
            client.Phone = updateClientDto.Phone;

            await _clientsStore.Save(client);

            return NoContent();
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _clientsStore.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            await _clientsStore.Delete(id);

            return NoContent();
        }
    }
} 