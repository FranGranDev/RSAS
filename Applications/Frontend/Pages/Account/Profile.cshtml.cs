using Application.DTOs;
using Frontend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly IAuthStateService _authStateService;

    public ProfileModel(IAuthStateService authStateService)
    {
        _authStateService = authStateService;
    }

    public UserDto CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            CurrentUser = await _authStateService.GetCurrentUserAsync();
            if (CurrentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }
            return Page();
        }
        catch
        {
            return RedirectToPage("/Account/Login");
        }
    }
} 