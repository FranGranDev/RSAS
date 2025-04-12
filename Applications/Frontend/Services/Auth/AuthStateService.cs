using Application.DTOs;
using Microsoft.AspNetCore.Authentication;

namespace Frontend.Services.Auth;

public interface IAuthStateService
{
    Task<bool> IsAuthenticatedAsync();
    Task<UserDto> GetCurrentUserAsync();
    Task<bool> IsInRoleAsync(string role);
}

public class AuthStateService : IAuthStateService
{
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private UserDto _currentUser;
    private bool _isInitialized;

    public AuthStateService(IAuthService authService, IHttpContextAccessor httpContextAccessor)
    {
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
        return _currentUser != null;
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
        return _currentUser;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
        return _currentUser?.Roles.Contains(role) ?? false;
    }

    private async Task InitializeAsync()
    {
        var token = await _httpContextAccessor.HttpContext?.GetTokenAsync("Token");
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                _currentUser = await _authService.GetCurrentUserAsync();
            }
            catch
            {
                _currentUser = null;
            }
        }
        _isInitialized = true;
    }
} 