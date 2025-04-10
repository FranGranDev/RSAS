using System.Text.Json;
using Frontend.Models;
using Microsoft.AspNetCore.Http;

namespace Frontend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TokenKey = "auth_token";

        public AuthService(IApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var response = await _apiService.PostAsync<AuthResponse>("auth/login", request);
            if (response.Success)
            {
                _apiService.SetToken(response.Token);
                SetTokenCookie(response.Token);
            }
            return response;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var response = await _apiService.PostAsync<AuthResponse>("auth/register", request);
            if (response.Success)
            {
                _apiService.SetToken(response.Token);
                SetTokenCookie(response.Token);
            }
            return response;
        }

        public void Logout()
        {
            _apiService.SetToken(null);
            RemoveTokenCookie();
        }

        public string GetToken()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[TokenKey];
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(TokenKey, token, cookieOptions);
        }

        private void RemoveTokenCookie()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(TokenKey);
        }
    }
} 