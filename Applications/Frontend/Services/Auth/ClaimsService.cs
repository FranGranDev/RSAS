using System.Security.Claims;
using Application.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Frontend.Services.Auth;

public interface IClaimsService
{
    ClaimsPrincipal CreateClaimsPrincipal(UserDto user);
}

public class ClaimsService : IClaimsService
{
    public ClaimsPrincipal CreateClaimsPrincipal(UserDto user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
} 