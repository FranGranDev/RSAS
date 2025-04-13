using System.Net;
using System.Net.Http.Json;
using Application.Data;
using Application.DTOs;
using Application.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Server.Tests.Controllers;

public class AuthControllerTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Логируем ответ для отладки
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response Status: {response.StatusCode}");
        Console.WriteLine($"Response Content: {responseContent}");

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(registerDto.Email);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, AppConst.Roles.Client);

        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(loginDto.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "wrong@example.com",
            Password = "WrongPass123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
        result.Token.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ShouldReturnUserInfo()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, AppConst.Roles.Client);

        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResult!.Token}");

        // Act
        var response = await Client.GetAsync("/api/auth/me");
        var result = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task ChangePassword_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, AppConst.Roles.Client);

        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResult!.Token}");

        var changePasswordDto = new ChangePasswordDto
        {
            CurrentPassword = "Test123!",
            NewPassword = "NewTest123!",
            ConfirmNewPassword = "NewTest123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", changePasswordDto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Message.Should().Be("Пароль успешно изменен");

        // Проверяем, что новый пароль работает
        var newLoginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "NewTest123!"
        };

        var newLoginResponse = await Client.PostAsJsonAsync("/api/auth/login", newLoginDto);
        newLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidCurrentPassword_ShouldReturnError()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, AppConst.Roles.Client);

        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResult!.Token}");

        var changePasswordDto = new ChangePasswordDto
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewTest123!",
            ConfirmNewPassword = "NewTest123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", changePasswordDto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("Неверный текущий пароль");
    }

    [Fact]
    public async Task ChangePassword_WithInvalidNewPassword_ShouldReturnError()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, AppConst.Roles.Client);

        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResult!.Token}");

        var changePasswordDto = new ChangePasswordDto
        {
            CurrentPassword = "Test123!",
            NewPassword = "short",
            ConfirmNewPassword = "short"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", changePasswordDto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
    }
}