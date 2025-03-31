using Application.Areas.Identity.Data;
using Application.Services;
using Microsoft.AspNetCore.Identity;

namespace Application.Data
{
    public class AppDbConfigure
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Client", "Company", "Admin", "Manager" };

            foreach (string roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    IdentityRole role = new IdentityRole { Name = roleName };
                    IdentityResult result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{roleName}'.");
                    }
                }
            }
        }
        public static async Task SeedUsers(UserManager<AppUser> userManager, IEmployeeStore employeeStore)
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
                    Role = "Администратор системы",
                };

                IdentityResult result = userManager.CreateAsync(user, "Admin123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                    userManager.AddToRoleAsync(user, "Manager").Wait();

                    employeeStore.Save(user, employee);
                }
            }
            else
            {
                var admin = userManager.FindByNameAsync("Admin@gmail.com").Result;
                userManager.AddToRoleAsync(admin, "Admin").Wait();
                userManager.AddToRoleAsync(admin, "Manager").Wait();

                if(employeeStore.Get(admin) == null)
                {
                    Employee employee = new Employee
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        Phone = "-",
                        Role = "Администратор системы",
                    };

                    employeeStore.Save(admin, employee);
                }
            }
        }
    }
}
