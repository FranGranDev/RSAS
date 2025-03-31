using Application.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Identity.Pages.Account.About
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;



        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if(await userManager.IsInRoleAsync(user, "Client"))
            {
                return RedirectToPage("Client");
            }
            else if(await userManager.IsInRoleAsync(user, "Company"))
            {
                return RedirectToPage("Company");
            }
            else if(await userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToPage("Employee");
            }
            else if (await userManager.IsInRoleAsync(user, "Manager"))
            {
                return RedirectToPage("Employee");
            }

            return NotFound();
        }
    }
}
