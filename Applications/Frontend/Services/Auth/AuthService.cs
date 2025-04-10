using Frontend.Models.Auth;
using Frontend.Services.Api;

namespace Frontend.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;

    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<AuthResponseViewModel> LoginAsync(LoginViewModel viewModel)
    {
        var response = await _apiService.PostAsync<AuthResponseViewModel>("auth/login", viewModel);
        if (response.Success)
        {
            _apiService.SetToken(response.Token);
        }
        return response;
    }

    public async Task<AuthResponseViewModel> RegisterAsync(RegisterViewModel viewModel)
    {
        var response = await _apiService.PostAsync<AuthResponseViewModel>("auth/register", viewModel);
        if (response.Success)
        {
            _apiService.SetToken(response.Token);
        }
        return response;
    }

    public async Task LogoutAsync()
    {
        _apiService.ClearToken();
    }

    public async Task<UserViewModel> GetCurrentUserAsync()
    {
        return await _apiService.GetAsync<UserViewModel>("auth/me");
    }
} 