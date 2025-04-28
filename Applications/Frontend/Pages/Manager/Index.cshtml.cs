using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
} 