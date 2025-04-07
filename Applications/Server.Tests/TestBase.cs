using Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Application.Data;
using Microsoft.AspNetCore.Identity;
using Application.Models;
using Xunit.Abstractions;

namespace Server.Tests;

public class TestBase : IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly AppDbContext Context;
    protected readonly UserManager<AppUser> UserManager;
    protected readonly RoleManager<IdentityRole> RoleManager;
    protected readonly ITestOutputHelper _output;

    public TestBase(ITestOutputHelper output)
    {
        _output = output;
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    // Заменяем реальную БД на InMemory
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var projectDir = Directory.GetCurrentDirectory();
                    var configPath = Path.Combine(projectDir, "appsettings.Testing.json");
                    config.AddJsonFile(configPath, optional: false);
                });
            });

        Client = Factory.CreateClient();
        Scope = Factory.Services.CreateScope();
        Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Инициализация тестовой БД
        Context.Database.EnsureCreated();
        
        // Инициализация ролей
        InitializeRoles().Wait();
    }

    private async Task InitializeRoles()
    {
        // Создаем роли, если они не существуют
        string[] roles = { "Admin", "Manager", "User" };
        foreach (var role in roles)
        {
            if (!await RoleManager.RoleExistsAsync(role))
            {
                await RoleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Scope.Dispose();
        Client.Dispose();
        Factory.Dispose();
    }
} 