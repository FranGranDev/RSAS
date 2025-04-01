using Application.Areas.Identity.Data;
using Application.DTOs;
using Application.Exceptions;
using Application.Services.Repository;
using AutoMapper;

namespace Application.Services.Clients
{
    public class ClientService : IClientService
    {
        private readonly IClientsStore _clientsStore;
        private readonly IMapper _mapper;

        public ClientService(
            IClientsStore clientsStore,
            IMapper mapper)
        {
            _clientsStore = clientsStore;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            var clients = await _clientsStore.GetAllAsync();
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ClientDto> GetClientByIdAsync(int id)
        {
            var client = await _clientsStore.GetByIdAsync(id);
            if (client == null)
            {
                throw new BusinessException($"Клиент с ID {id} не найден");
            }

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto, string userId)
        {
            // Проверяем, не существует ли уже клиент с таким UserId
            var existingClient = await _clientsStore.GetByUserIdAsync(userId);
            if (existingClient != null)
            {
                throw new BusinessException("Клиент с таким UserId уже существует");
            }

            var client = _mapper.Map<Client>(createClientDto);
            client.UserId = userId;
            await _clientsStore.SaveAsync(client);
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> UpdateClientAsync(int id, UpdateClientDto updateClientDto)
        {
            var client = await _clientsStore.GetByIdAsync(id);
            if (client == null)
            {
                throw new BusinessException($"Клиент с ID {id} не найден");
            }

            _mapper.Map(updateClientDto, client);
            await _clientsStore.SaveAsync(client);
            return _mapper.Map<ClientDto>(client);
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _clientsStore.GetByIdAsync(id);
            if (client == null)
            {
                throw new BusinessException($"Клиент с ID {id} не найден");
            }

            await _clientsStore.DeleteAsync(id);
        }

        public async Task<ClientDto> GetClientByUserIdAsync(string userId)
        {
            var client = await _clientsStore.GetByUserIdAsync(userId);
            if (client == null)
            {
                throw new BusinessException($"Клиент с UserId {userId} не найден");
            }

            return _mapper.Map<ClientDto>(client);
        }
    }
}