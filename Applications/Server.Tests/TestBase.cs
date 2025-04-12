using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application;
using Application.Data;
using Application.DTOs;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Server.Tests;

public class TestBase : IDisposable
{
    private readonly string _dbName;
    protected readonly ITestOutputHelper _output;
    protected readonly HttpClient Client;
    protected readonly AppDbContext Context;
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly RoleManager<IdentityRole> RoleManager;
    protected readonly IServiceScope Scope;
    protected readonly UserManager<AppUser> UserManager;

    public TestBase(ITestOutputHelper output)
    {
        _output = output;
        _dbName = $"TestDb_{Guid.NewGuid()}";

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    // Заменяем реальную БД на InMemory
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase(_dbName); });

                    // Отключаем автоматическую инициализацию данных
                    services.Configure<IdentityOptions>(options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.SignIn.RequireConfirmedEmail = false;
                        options.SignIn.RequireConfirmedPhoneNumber = false;
                    });
                });

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var projectDir = Directory.GetCurrentDirectory();
                    var configPath = Path.Combine(projectDir, "appsettings.Testing.json");
                    config.AddJsonFile(configPath, false);
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

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Scope.Dispose();
        Client.Dispose();
        Factory.Dispose();
    }

    private async Task InitializeRoles()
    {
        // Создаем роли, если они не существуют
        string[] roles = { AppConst.Roles.Manager, AppConst.Roles.Client };
        foreach (var role in roles)
            if (!await RoleManager.RoleExistsAsync(role))
                await RoleManager.CreateAsync(new IdentityRole(role));
    }

    protected async Task<(string Token, string UserId)> LoginAsManager()
    {
        // Создаем менеджера
        var manager = new AppUser
        {
            UserName = "manager@test.com",
            Email = "manager@test.com"
        };
        await UserManager.CreateAsync(manager, "Test123!");
        await UserManager.AddToRoleAsync(manager, "Manager");

        // Выполняем вход
        var loginDto = new LoginDto
        {
            Email = "manager@test.com",
            Password = "Test123!"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);

        return (result.Token, manager.Id);
    }

    protected async Task<(string token, string userId)> LoginAsUser()
    {
        // Создаем обычного пользователя
        var user = new AppUser
        {
            UserName = $"user{DateTime.Now.Ticks}@test.com",
            Email = $"user{DateTime.Now.Ticks}@test.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, AppConst.Roles.Client);

        // Выполняем вход как пользователь
        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        return (loginResult.Token, user.Id);
    }
}