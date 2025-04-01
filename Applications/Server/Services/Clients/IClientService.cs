using Application.DTOs;

namespace Application.Services.Clients
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task<ClientDto> GetClientByIdAsync(string userId);
        Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto, string userId);
        Task<ClientDto> UpdateClientAsync(string userId, UpdateClientDto updateClientDto);
        Task DeleteClientAsync(string userId);
        Task<ClientDto> GetClientByPhoneAsync(string phone);
        Task<ClientDto> GetClientByNameAsync(string firstName, string lastName);
        Task<bool> ExistsByPhoneAsync(string phone);
    }
}