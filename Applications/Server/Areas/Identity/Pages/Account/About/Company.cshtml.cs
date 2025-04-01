using Application.Areas.Identity.Data;
using Application.Models;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Identity.Pages.Account.About
{
    [BindProperties]
    [Authorize(Roles = "Company")]
    public class CompanyModel : PageModel
    {
        private readonly ICompanyStore companyStore;

        private readonly UserManager<AppUser> userManager;

        public CompanyModel(UserManager<AppUser> userManager, ICompanyStore companyStore)
        {
            this.userManager = userManager;
            this.companyStore = companyStore;
        }


        public CompanyViewModel Company { get; set; }
        public ChangePasswordViewModel Password { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Company company = companyStore.Get(user);

            Company = new()
            {
                Name = company.Name,
                Inn = company.Inn,
                Kpp = company.Kpp,
                BankName = company.BankName,
                BankBic = company.BankBic,
                BankAccount = company.BankAccount,
                Email = company.Email,
                Phone = company.Phone
            };
            Password = new ChangePasswordViewModel();

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

            if (!TryValidateModel(Company))
            {
                return Page();
            }

            var company = new Company
            {
                Name = Company.Name,
                Inn = Company.Inn,
                Kpp = Company.Kpp,
                BankName = Company.BankName,
                BankBic = Company.BankBic,
                BankAccount = Company.BankAccount,
                Email = Company.Email,
                Phone = Company.Phone
            };
            companyStore.Save(user, company);

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