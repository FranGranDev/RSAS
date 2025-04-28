using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Services.Auth;

namespace Frontend.Pages;

public class IndexModel : PageModel
{
    private readonly IAuthService _authService;

    public IndexModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/Account/Login");
        }

        if (await _authService.IsInRoleAsync("Client"))
        {
            return RedirectToPage("/Client/Index");
        }

        if (await _authService.IsInRoleAsync("Manager"))
        {
            return RedirectToPage("/Manager/Index");
        }

        return Page();
    }
}
