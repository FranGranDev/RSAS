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

namespace Application.Areas.Identity.Pages.Account
{
    [BindProperties]
    public class RegisterCompanyModel : PageModel
    {
        private const string Role = "Company";

        public RegisterCompanyModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            ICompanyStore companyStore)
        {
            this.userManager = userManager;
            this.userStore = userStore;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.companyStore = companyStore;
            this.userEmailStore = (IUserEmailStore<AppUser>)userStore;
        }

        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserStore<AppUser> userStore;
        private readonly IUserEmailStore<AppUser> userEmailStore;
        private readonly ICompanyStore companyStore;



        public CompanyViewModel Company { get; set; }
        public PasswordViewModel Password { get; set; }        
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Company = new();
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
                Company company = new Company
                {
                    Name = Company.Name,
                    Inn = Company.Inn,
                    Kpp = Company.Kpp,
                    BankName = Company.BankName,
                    BankBic = Company.BankBic,
                    BankAccount = Company.BankAccount,
                    Email = Company.Email,
                    Phone = Company.Phone,
                };

                await userStore.SetUserNameAsync(user, Company.Email, CancellationToken.None);

                await userEmailStore.SetEmailAsync(user, Company.Email, CancellationToken.None);

                user.LockoutEnabled = false;
                user.PhoneNumberConfirmed = true;
                user.EmailConfirmed = true;


                var result = await userManager.CreateAsync(user, Password.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Role);
                    companyStore.Save(user, company);

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
