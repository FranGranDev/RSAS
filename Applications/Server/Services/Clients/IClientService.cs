using Application.DTOs;

namespace Application.Services.Clients
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task<ClientDto> GetClientByIdAsync(int id);
        Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto, string userId);
        Task<ClientDto> UpdateClientAsync(int id, UpdateClientDto updateClientDto);
        Task DeleteClientAsync(int id);
        Task<ClientDto> GetClientByUserIdAsync(string userId);
    }
}