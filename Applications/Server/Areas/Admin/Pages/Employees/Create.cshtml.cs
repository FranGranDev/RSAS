using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Application.Areas.Identity.Data;
using Application.Services.Repository;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Areas.Admin.Pages.Employees
{
    [Authorize(Roles = "Admin")]
    [BindProperties(SupportsGet = true)]
    public class CreateModel : PageModel
    {
        private readonly IEmployeeStore employeeStore;

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public CreateModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
            IEmployeeStore employeeStore)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.employeeStore = employeeStore;
        }


        public EmployeeViewModel Client { get; set; }

        [DisplayName("Временный пароль")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [StringLength(100, ErrorMessage = "{0} должен быть как минимум {2} и максимум {1} символов.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string TempPassword { get; set; }

        [Required] [DisplayName("Должность")] public string Role { get; set; }

        public IEnumerable<SelectListItem> RoleList =>
            new List<SelectListItem>
            {
                new()
                {
                    Text = "Директор",
                    Value = "Admin"
                },
                new()
                {
                    Text = "Менеджер",
                    Value = "Manager"
                }
            };


        public IActionResult OnGet()
        {
            TempPassword = "Bsuir123";

            ModelState.Clear();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Client.Id");

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Заполните все поля для создания сотрудника.";
                return Page();
            }

            var user = new AppUser
            {
                UserName = Client.Email,
                Email = Client.Email,
                NormalizedUserName = Client.Email.ToUpper(),
                NormalizedEmail = Client.Email.ToUpper()
            };
            var client = new Employee
            {
                FirstName = Client.FirstName,
                LastName = Client.LastName,
                Phone = Client.Phone,
                Role = Client.Role
            };

            var result = await userManager.CreateAsync(user, TempPassword);
            if (result.Succeeded)
            {
                employeeStore.Save(user, client);
                await userManager.AddToRoleAsync(user, Role);
            }
            else
            {
                TempData["error"] = "Сотрудник с такой эл.почтой уже существует.";
                return Page();
            }

            TempData["success"] = "Сотрудник создан";

            return RedirectToPage("Index");
        }
    }
}