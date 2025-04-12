using Application.DTOs;
using Frontend.Services.Api;

namespace Frontend.Services.Account;

public class ClientService : IClientService
{
    private readonly IApiService _apiService;

    public ClientService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<ClientDto> GetCurrentClientAsync()
    {
        return await _apiService.GetAsync<ClientDto>("/api/clients/current");
    }

    public async Task<bool> ExistsForCurrentUserAsync()
    {
        return await _apiService.GetAsync<bool>("/api/clients/exists");
    }

    public async Task<ClientDto> CreateForCurrentUserAsync(CreateClientDto createClientDto)
    {
        return await _apiService.PostAsync<ClientDto, CreateClientDto>("/api/clients/create-for-current", createClientDto);
    }

    public async Task<ClientDto> UpdateSelfAsync(UpdateClientDto updateClientDto)
    {
        return await _apiService.PutAsync<ClientDto, UpdateClientDto>("/api/clients/update-self", updateClientDto);
    }
} 