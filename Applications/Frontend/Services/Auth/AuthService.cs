using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Frontend.Services.Auth;

public class AuthService : IAuthService
{
    private UserDto _currentUser;
    private bool _isInitialized;
    
    private readonly IApiService _apiService;
    private readonly IClaimsService _claimsService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IApiService apiService,
        IClaimsService claimsService,
        IHttpContextAccessor httpContextAccessor)
    {
        _apiService = apiService;
        _claimsService = claimsService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto model)
    {
        var response = await _apiService.PostAsync<AuthResponseDto, LoginDto>("api/auth/login", model);
        if (response.Success)
        {
            await SignInAsync(response.User, response.Token);
        }
        return response;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
    {
        var response = await _apiService.PostAsync<AuthResponseDto, RegisterDto>("api/auth/register", model);
        if (response.Success)
        {
            await SignInAsync(response.User, response.Token);
        }
        return response;
    }

    public async Task LogoutAsync()
    {
        await _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        return await _apiService.GetAsync<UserDto>("api/auth/me");
    }

    private async Task SignInAsync(UserDto user, string token)
    {
        var principal = _claimsService.CreateClaimsPrincipal(user);
        
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            IssuedUtc = DateTimeOffset.UtcNow
        };

        authProperties.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "Token", Value = token }
        });

        await _httpContextAccessor.HttpContext?.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
        return _currentUser != null;
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
                _currentUser = await GetCurrentUserAsync();
                _isInitialized = true;
            }
            catch
            {
                _currentUser = null;
            }
        }
    }
} 