using Application.Areas.Identity.Data;
using Application.Services;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Application.Models;
using Application.Services.Repository;

namespace Application.Areas.Identity.Pages.Account
{
    [BindProperties]
    public class RegisterClientModel : PageModel
    {
        private const string Role = "Client";

        public RegisterClientModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IClientsStore clientsStore)
        {
            this.userManager = userManager;
            this.userStore = userStore;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.clientsStore = clientsStore;
            this.userEmailStore = (IUserEmailStore<AppUser>)userStore;
        }

        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserStore<AppUser> userStore;
        private readonly IUserEmailStore<AppUser> userEmailStore;
        private readonly IClientsStore clientsStore;



        public ClientViewModel Client { get; set; }
        public PasswordViewModel Password { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Client = new();
            Password = new();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ModelState.Remove("returnUrl");

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                AppUser user = CreateUser();
                Client client = new Client
                {
                    FirstName = Client.FirstName,
                    LastName = Client.LastName,
                    Phone = Client.Phone,
                    UserId = user.Id,
                };

                await userStore.SetUserNameAsync(user, Client.Email, CancellationToken.None);
               
                await userEmailStore.SetEmailAsync(user, Client.Email, CancellationToken.None);

                user.LockoutEnabled = false;
                user.PhoneNumberConfirmed = true;
                user.EmailConfirmed = true;


                var result = await userManager.CreateAsync(user, Password.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Role);
                    clientsStore.Save(user, client);

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }


        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
