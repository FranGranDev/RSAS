using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using AutoMapper;
using Server.Services.Repository;

namespace Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(
            IClientRepository clientRepository,
            IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            var clients = await _clientRepository.GetAllWithUserAsync();
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ClientDto> GetClientByIdAsync(string userId)
        {
            var client = await _clientRepository.GetWithUserAsync(userId);
            if (client == null)
            {
                throw new BusinessException($"Клиент с ID {userId} не найден");
            }

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto, string userId)
        {
            // Проверяем, не существует ли уже клиент с таким UserId
            var existingClient = await _clientRepository.GetWithUserAsync(userId);
            if (existingClient != null)
            {
                throw new BusinessException("Клиент с таким UserId уже существует");
            }

            // Проверяем, не существует ли уже клиент с таким телефоном
            if (await _clientRepository.ExistsByPhoneAsync(createClientDto.Phone))
            {
                throw new BusinessException("Клиент с таким номером телефона уже существует");
            }

            var client = _mapper.Map<Client>(createClientDto);
            client.UserId = userId;
            await _clientRepository.AddAsync(client);
            
            // Получаем клиента с User для корректного маппинга
            client = await _clientRepository.GetWithUserAsync(userId);
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> UpdateClientAsync(string userId, UpdateClientDto updateClientDto)
        {
            var client = await _clientRepository.GetWithUserAsync(userId);
            if (client == null)
            {
                throw new BusinessException($"Клиент с ID {userId} не найден");
            }

            // Проверяем, не занят ли новый номер телефона другим клиентом
            if (client.Phone != updateClientDto.Phone &&
                await _clientRepository.ExistsByPhoneAsync(updateClientDto.Phone))
            {
                throw new BusinessException("Клиент с таким номером телефона уже существует");
            }

            _mapper.Map(updateClientDto, client);
            await _clientRepository.UpdateAsync(client);
            
            // Получаем обновленного клиента с User для корректного маппинга
            client = await _clientRepository.GetWithUserAsync(userId);
            return _mapper.Map<ClientDto>(client);
        }

        public async Task DeleteClientAsync(string userId)
        {
            var client = await _clientRepository.GetWithUserAsync(userId);
            if (client == null)
            {
                throw new BusinessException($"Клиент с ID {userId} не найден");
            }

            await _clientRepository.DeleteAsync(userId);
        }

        public async Task<ClientDto> GetClientByPhoneAsync(string phone)
        {
            var client = await _clientRepository.GetByPhoneWithUserAsync(phone);
            if (client == null)
            {
                throw new BusinessException($"Клиент с телефоном {phone} не найден");
            }

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> GetClientByNameAsync(string firstName, string lastName)
        {
            var clients = await _clientRepository.GetByNameWithUserAsync(firstName, lastName);
            var client = clients.FirstOrDefault();
            if (client == null)
            {
                throw new BusinessException($"Клиент с именем {firstName} {lastName} не найден");
            }

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _clientRepository.ExistsByPhoneAsync(phone);
        }
    }
}