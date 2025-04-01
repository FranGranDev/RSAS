using Application.Models;
using Microsoft.AspNetCore.Identity;
using Server.Services.Repository;

namespace Application.Data
{
    public class AppDbConfigure
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Client", "Company", "Admin", "Manager" };

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
            if (userManager.FindByNameAsync("Admin@gmail.com").Result == null)
            {
                AppUser user = new AppUser
                {
                    UserName = "Admin@gmail.com",
                    Email = "Admin@gmail.com"
                };
                Employee employee = new Employee
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    Phone = "-",
                    Role = "Администратор системы"
                };

                var result = userManager.CreateAsync(user, "Admin123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                    userManager.AddToRoleAsync(user, "Manager").Wait();

                    employee.UserId = user.Id;
                    await employeeRepository.AddAsync(employee);
                }
            }
            else
            {
                var admin = userManager.FindByNameAsync("Admin@gmail.com").Result;
                userManager.AddToRoleAsync(admin, "Admin").Wait();
                userManager.AddToRoleAsync(admin, "Manager").Wait();

                if (await employeeRepository.GetByIdAsync(admin.Id) == null)
                {
                    Employee employee = new Employee
                    {
                        UserId = admin.Id,
                        FirstName = "Admin",
                        LastName = "Admin",
                        Phone = "-",
                        Role = "Администратор системы"
                    };

                    await employeeRepository.AddAsync(employee);
                }
            }
        }
    }
}