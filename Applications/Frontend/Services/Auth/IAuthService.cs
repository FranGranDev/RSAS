using Application.DTOs;

namespace Frontend.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto model);
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);
    Task LogoutAsync();
    Task<UserDto> GetCurrentUserAsync();
} 