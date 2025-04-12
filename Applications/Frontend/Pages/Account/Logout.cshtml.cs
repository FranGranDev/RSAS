using Frontend.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly IAuthService _authService;

    public LogoutModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await _authService.LogoutAsync();
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
} 