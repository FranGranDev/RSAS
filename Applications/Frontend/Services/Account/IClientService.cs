using Application.DTOs;

namespace Frontend.Services.Account;

public interface IClientService
{
    Task<ClientDto> GetCurrentClientAsync();
    Task<bool> ExistsForCurrentUserAsync();
    Task<ClientDto> CreateForCurrentUserAsync(CreateClientDto createClientDto);
    Task<ClientDto> UpdateSelfAsync(UpdateClientDto updateClientDto);
    Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordDto dto);
} 