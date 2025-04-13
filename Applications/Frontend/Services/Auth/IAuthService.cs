using Application.DTOs;

namespace Frontend.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto model);
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);
    Task<UserDto> GetCurrentUserAsync();
    Task<bool> IsInRoleAsync(string role);
    Task<bool> IsAuthenticatedAsync();
    Task LogoutAsync();
} 