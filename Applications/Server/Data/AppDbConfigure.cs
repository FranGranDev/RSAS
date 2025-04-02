using Application.Models;
using Microsoft.AspNetCore.Identity;
using Server.Services.Repository;

namespace Application.Data
{
    public class AppDbConfigure
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Client", "Manager" };

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
            if (userManager.FindByNameAsync("Manager@gmail.com").Result == null)
            {
                AppUser user = new AppUser
                {
                    UserName = "Manager@gmail.com",
                    Email = "Manager@gmail.com"
                };
                Employee employee = new Employee
                {
                    FirstName = "Manager",
                    LastName = "Manager",
                    Phone = "-",
                    Role = "Менеджер системы"
                };

                var result = userManager.CreateAsync(user, "Manager123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Manager").Wait();

                    employee.UserId = user.Id;
                    await employeeRepository.AddAsync(employee);
                }
            }
            else
            {
                var manager = userManager.FindByNameAsync("Manager@gmail.com").Result;
                userManager.AddToRoleAsync(manager, "Manager").Wait();

                if (await employeeRepository.GetByIdAsync(manager.Id) == null)
                {
                    Employee employee = new Employee
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
}