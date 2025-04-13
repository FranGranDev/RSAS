using Application.DTOs;
using Frontend.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public LoginDto Input { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var response = await _authService.LoginAsync(Input);
            if (response.Success)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return Page();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Неверный email или пароль");
            return Page();
        }
    }
} 