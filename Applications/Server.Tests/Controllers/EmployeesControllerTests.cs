using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs;
using Application.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Server.Tests.Controllers;

public class EmployeesControllerTests(ITestOutputHelper output) : TestBase(output)
{
    private async Task<(string Token, string UserId)> LoginAsManager()
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

    [Fact]
    public async Task GetEmployees_AsAdmin_ShouldReturnEmployees()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/employees");
        var employees = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        employees.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEmployees_AsManager_ShouldReturnEmployees()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/employees");
        var employees = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        employees.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEmployees_WithoutAdminOrManagerRole_ShouldReturnForbidden()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "user@test.com",
            Email = "user@test.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, "User");

        var loginDto = new LoginDto
        {
            Email = "user@test.com",
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Act
        var response = await Client.GetAsync("/api/employees");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateEmployee_AsManager_ShouldCreateEmployee()
    {
        // Arrange
        await LoginAsManager();

        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = "employee@test.com",
            Phone = "+375291234567",
            Role = "Manager"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/employees", createEmployeeDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var employee = JsonSerializer.Deserialize<EmployeeDto>(responseContent, options);
            employee.Should().NotBeNull();
            employee!.FirstName.Should().Be(createEmployeeDto.FirstName);
            employee.LastName.Should().Be(createEmployeeDto.LastName);
            employee.Email.Should().Be(createEmployeeDto.Email);
            employee.Phone.Should().Be(createEmployeeDto.Phone);
            employee.Role.Should().Be(createEmployeeDto.Role);
        }
    }

    [Fact]
    public async Task CreateEmployee_AsUser_ShouldReturnForbidden()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "user@test.com",
            Email = "user@test.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, "User");

        var loginDto = new LoginDto
        {
            Email = "user@test.com",
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = "employee@test.com",
            Phone = "+375291234567",
            Role = "Manager"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/employees", createEmployeeDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEmployee_WithValidId_ShouldReturnEmployee()
    {
        // Arrange
        await LoginAsManager();

        // Создаем сотрудника с уникальным email
        var uniqueEmail = $"employee{DateTime.Now.Ticks}@test.com";
        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = uniqueEmail,
            Phone = "+375291234567",
            Role = "Manager"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/employees", createEmployeeDto);
        var createdEmployee = await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();

        // Act
        var response = await Client.GetAsync($"/api/employees/email/{createdEmployee!.Email}");
        var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        employee.Should().NotBeNull();
        employee!.FirstName.Should().Be(createEmployeeDto.FirstName);
        employee.LastName.Should().Be(createEmployeeDto.LastName);
        employee.Email.Should().Be(createEmployeeDto.Email);
        employee.Phone.Should().Be(createEmployeeDto.Phone);
        employee.Role.Should().Be(createEmployeeDto.Role);
    }

    [Fact]
    public async Task GetEmployee_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsManager();
        var invalidEmail = "invalid-email@test.com";

        // Act
        var response = await Client.GetAsync($"/api/employees/email/{invalidEmail}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateEmployee_AsManager_ShouldUpdateEmployee()
    {
        // Arrange
        await LoginAsManager();

        // Создаем сотрудника с уникальным email
        var uniqueEmail = $"employee{DateTime.Now.Ticks}@test.com";
        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = uniqueEmail,
            Phone = "+375291234567",
            Role = "Manager"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/employees", createEmployeeDto);
        var createdEmployee = await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();

        var updateEmployeeDto = new UpdateEmployeeDto
        {
            FirstName = "Updated",
            LastName = "Employee",
            Email = uniqueEmail,
            Phone = "+375291234568",
            Role = "Senior Manager"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/employees/email/{uniqueEmail}", updateEmployeeDto);
        var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        employee.Should().NotBeNull();
        employee!.FirstName.Should().Be(updateEmployeeDto.FirstName);
        employee.LastName.Should().Be(updateEmployeeDto.LastName);
        employee.Email.Should().Be(updateEmployeeDto.Email);
        employee.Phone.Should().Be(updateEmployeeDto.Phone);
        employee.Role.Should().Be(updateEmployeeDto.Role);
    }

    [Fact]
    public async Task DeleteEmployee_AsManager_ShouldDeleteEmployee()
    {
        // Arrange
        await LoginAsManager();

        // Создаем сотрудника с уникальным email
        var uniqueEmail = $"employee{DateTime.Now.Ticks}@test.com";
        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = uniqueEmail,
            Phone = "+375291234567",
            Role = "Manager"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/employees", createEmployeeDto);
        var createdEmployee = await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();

        // Act
        var response = await Client.DeleteAsync($"/api/employees/email/{createdEmployee!.Email}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Проверяем, что сотрудник действительно удален
        var getResponse = await Client.GetAsync($"/api/employees/email/{createdEmployee.Email}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEmployeesByRole_AsManager_ShouldReturnEmployees()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/employees/role/Manager");
        var employees = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        employees.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEmployeeByPhone_WithValidPhone_ShouldReturnEmployee()
    {
        // Arrange
        await LoginAsManager();

        // Создаем сотрудника с уникальным email
        var uniqueEmail = $"employee{DateTime.Now.Ticks}@test.com";
        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = uniqueEmail,
            Phone = "+375291234567",
            Role = "Manager"
        };

        await Client.PostAsJsonAsync("/api/employees", createEmployeeDto);

        // Act
        var response = await Client.GetAsync($"/api/employees/phone/{createEmployeeDto.Phone}");
        var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        employee.Should().NotBeNull();
        employee!.Phone.Should().Be(createEmployeeDto.Phone);
    }

    [Fact]
    public async Task GetEmployeeByName_WithValidName_ShouldReturnEmployee()
    {
        // Arrange
        await LoginAsManager();

        // Создаем сотрудника с уникальным email
        var uniqueEmail = $"employee{DateTime.Now.Ticks}@test.com";
        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = uniqueEmail,
            Phone = "+375291234567",
            Role = "Manager"
        };

        await Client.PostAsJsonAsync("/api/employees", createEmployeeDto);

        // Act
        var response =
            await Client.GetAsync(
                $"/api/employees/name?firstName={createEmployeeDto.FirstName}&lastName={createEmployeeDto.LastName}");
        var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        employee.Should().NotBeNull();
        employee!.FirstName.Should().Be(createEmployeeDto.FirstName);
        employee.LastName.Should().Be(createEmployeeDto.LastName);
    }
}