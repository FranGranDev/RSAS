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
    [Authorize(Roles = "Manager,Admin")]
    public class EmployeeModel : PageModel
    {
        public EmployeeModel(UserManager<AppUser> userManager, IEmployeeStore employeeStore)
        {
            this.userManager = userManager;
            this.employeeStore = employeeStore;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly IEmployeeStore employeeStore;


        public EmployeeViewModel Client { get; set; }
        public ChangePasswordViewModel Password { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Employee client = employeeStore.Get(user);

            Client = new()
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Phone = client.Phone,
                Email = user.Email,
                Role = client.Role,
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

            Employee client = new Employee
            {
                FirstName = Client.FirstName,
                LastName = Client.LastName,
                Phone = Client.Phone,
                Role = Client.Role,
            };
            employeeStore.Save(user, client);

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
