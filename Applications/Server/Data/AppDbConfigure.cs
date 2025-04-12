using Application.Models;
using Microsoft.AspNetCore.Identity;
using Server.Services.Repository;

namespace Application.Data
{
    public class AppDbConfigure
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { AppConst.Roles.Manager, AppConst.Roles.Client };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{roleName}'.");
                    }
                }
            }
        }

        public static async Task SeedUsers(UserManager<AppUser> userManager, IEmployeeRepository employeeRepository)
        {
            var managerEmail = "Manager@gmail.com";
            var manager = await userManager.FindByEmailAsync(managerEmail);

            if (manager == null)
            {
                manager = new AppUser
                {
                    UserName = managerEmail,
                    Email = managerEmail
                };

                var result = await userManager.CreateAsync(manager, "Manager123");
                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create manager user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Проверяем, есть ли уже роль у пользователя
            var userRoles = await userManager.GetRolesAsync(manager);
            if (!userRoles.Contains("Manager"))
            {
                var roleResult = await userManager.AddToRoleAsync(manager, "Manager");
                if (!roleResult.Succeeded)
                {
                    throw new Exception(
                        $"Failed to add Manager role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }

            // Проверяем существование записи сотрудника
            var employee = await employeeRepository.GetByIdAsync(manager.Id);
            if (employee == null)
            {
                employee = new Employee
                {
                    UserId = manager.Id,
                    FirstName = "Manager",
                    LastName = "Manager",
                    Phone = "-",
                    Role = "Менеджер системы"
                };

                await employeeRepository.AddAsync(employee);
            }
        }
    }
}