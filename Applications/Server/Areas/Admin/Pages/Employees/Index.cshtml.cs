using Application.Areas.Identity.Data;
using Application.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Application.Areas.Admin.Pages.Employees
{
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class IndexModel : PageModel
    {
        public IndexModel(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        private readonly UserManager<AppUser> userManager;



        public List<EmployeeViewModel> Employees { get; set; }
        public string SelfId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Employees = await userManager
                .Users
                .Where(x => x.Employee != null)
                .Select(x => new EmployeeViewModel()
                {
                    Id = x.Id,
                    FirstName = x.Employee.FirstName,
                    LastName = x.Employee.LastName,
                    Phone = x.Employee.Phone,
                    Email = x.Email,
                    Role = x.Employee.Role,
                })
                .ToListAsync();
            var self = await userManager.GetUserAsync(User);
            SelfId = self.Id;

            return Page();
        }

        public async Task<IActionResult> OnPostDelete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user == null)
            {
                return RedirectToPage();

            }
            if(user.UserName == "Admin@gmail.com")
            {
                TempData["error"] = "���������� ������� ����� ������������";
                return RedirectToPage();
            }
            if(user == await userManager.GetUserAsync(User))
            {
                TempData["error"] = "�� �� ������ ������� ����";
                return RedirectToPage();
            }

            await userManager.DeleteAsync(user);

            TempData["success"] = "������������ ������� ������";

            return RedirectToPage();
        }
    }
}
