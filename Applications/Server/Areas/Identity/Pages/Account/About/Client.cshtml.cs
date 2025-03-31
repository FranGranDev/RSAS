using Application.Areas.Identity.Data;
using Application.Services;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Identity.Pages.Account.About
{
    [BindProperties]
    [Authorize(Roles = "Client")]
    public class ClientModel : PageModel
    {
        public ClientModel(UserManager<AppUser> userManager, IClientsStore clientsStore)
        {
            this.userManager = userManager;
            this.clientsStore = clientsStore;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly IClientsStore clientsStore;


        public ClientViewModel Client { get; set; }
        public ChangePasswordViewModel Password { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Client client = clientsStore.Get(user);

            Client = new()
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Phone = client.Phone,
                Email = user.Email,
            };
            Password = new();

            return Page();
        }

        public async Task<IActionResult> OnPostGeneral()
        {
            ModelState.Clear();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (!TryValidateModel(Client))
            {
                return Page();
            }

            Client client = new Client
            {
                FirstName = Client.FirstName,
                LastName = Client.LastName,
                Phone = Client.Phone,
            };
            clientsStore.Save(user, client);
 
            return Page();
        }
        public async Task<IActionResult> OnPostPassword()
        {
            ModelState.Clear();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (!TryValidateModel(Password))
            {
                return Page();
            }

            var result = await userManager.ChangePasswordAsync(user, Password.OldPassword, Password.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("password", "Invalid Password");
            }

            return RedirectToPage();
        }
    }
}
