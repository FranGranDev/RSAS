using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages;

[AllowAnonymous]
public class AccessDeniedModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
} 