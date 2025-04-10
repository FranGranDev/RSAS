using Frontend.Models;

namespace Frontend.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        void Logout();
        string GetToken();
    }
} 