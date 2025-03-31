using Application.Areas.Identity.Data;
using Application.Services;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Application.Areas.Admin.Pages.Employees
{
    [Authorize(Roles = "Admin")]
    [BindProperties(SupportsGet = true)]
    public class CreateModel : PageModel
    {
        public CreateModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IEmployeeStore employeeStore)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.employeeStore = employeeStore;
        }

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IEmployeeStore employeeStore;


        public EmployeeViewModel Client { get; set; }
        [DisplayName("��������� ������")]
        [Required(ErrorMessage = "���������� ��������� ���� {0}")]
        [StringLength(100, ErrorMessage = "{0} ������ ���� ��� ������� {2} � �������� {1} ��������.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string TempPassword { get; set; }
        [Required, DisplayName("���������")]
        public string Role { get; set; }
        public IEnumerable<SelectListItem> RoleList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem
                    {
                        Text = "��������",
                        Value = "Admin",
                    },
                    new SelectListItem
                    {
                        Text = "��������",
                        Value = "Manager",
                    },
                };
            }
        }



        public IActionResult OnGet()
        {
            TempPassword = "Bsuir123";

            ModelState.Clear();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Client.Id");

            if(!ModelState.IsValid)
            {
                TempData["error"] = "��������� ��� ���� ��� �������� ����������.";
                return Page();
            }

            AppUser user = new AppUser()
            {
                UserName = Client.Email,
                Email = Client.Email,
                NormalizedUserName = Client.Email.ToUpper(),
                NormalizedEmail = Client.Email.ToUpper(),                
            };
            Employee client = new Employee()
            {
                FirstName = Client.FirstName,
                LastName = Client.LastName,
                Phone = Client.Phone,
                Role = Client.Role,
            };

            var result = await userManager.CreateAsync(user, TempPassword);
            if(result.Succeeded)
            {
                employeeStore.Save(user, client);
                await userManager.AddToRoleAsync(user, Role);
            }
            else
            {
                TempData["error"] = "��������� � ����� ��.������ ��� ����������.";
                return Page();
            }

            TempData["success"] = "��������� ������";

            return RedirectToPage("Index");
        }
    }
}
