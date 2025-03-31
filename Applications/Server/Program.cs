using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Application.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Application.Data;
using Application.Services;
using Application.Data.Repository;
using System.Globalization;
using Application.MyTagHelpers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Configuration;

namespace Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContextConnection"));
            });
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
            });

            builder.Services.AddDefaultIdentity<AppUser>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddMemoryCache();


            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


            builder.Services.AddTransient<ShowIfRoleTagHelper>();

            builder.Services.AddTransient<IStockStore, EFStockStore>();
            builder.Services.AddTransient<IProductsStore, EFProductsStore>();
            builder.Services.AddTransient<IStockProductsStore, EFStockProductsStore>();
            builder.Services.AddTransient<IOrderStore, EFOrderStore>();
            builder.Services.AddTransient<ISalesStore, EFSalesStore>();
            builder.Services.AddTransient<DataManager>();


            builder.Services.AddTransient<IClientsStore, EFClientStore>();
            builder.Services.AddTransient<IEmployeeStore, EFEmployeeStore>();
            builder.Services.AddTransient<ICompanyStore, EFCompanyStore>();



            builder.Services.AddRazorPages();


            var app = builder.Build();

            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetService<UserManager<AppUser>>();
                var roleManager = services.GetService<RoleManager<IdentityRole>>();
                var employeeStore = services.GetService<IEmployeeStore>();
                await AppDbConfigure.SeedRoles(roleManager);
                await AppDbConfigure.SeedUsers(userManager, employeeStore);
            }


            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                var currentThreadCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                currentThreadCulture.NumberFormat = NumberFormatInfo.InvariantInfo;

                Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                Thread.CurrentThread.CurrentUICulture = currentThreadCulture;

                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            app.MapRazorPages();

            app.Run();
        }
    }
}