using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Frontend.Services.Auth;

public class AuthService : IAuthService
{
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
        var response = await _apiService.PostAsync<AuthResponseDto>("api/auth/login", model);
        if (response.Success)
        {
            await SignInAsync(response.User, response.Token);
        }
        return response;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
    {
        var response = await _apiService.PostAsync<AuthResponseDto>("api/auth/register", model);
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
} 