using Frontend.Models.Auth;

namespace Frontend.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseViewModel> LoginAsync(LoginViewModel model);
    Task<AuthResponseViewModel> RegisterAsync(RegisterViewModel viewModel);
    Task LogoutAsync();
    Task<UserViewModel> GetCurrentUserAsync();
} 