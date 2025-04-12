using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Data;
using Application.DTOs;
using Application.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Server.Tests.Controllers;

public class ClientsControllerTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public async Task GetClients_AsManager_ShouldReturnClients()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/clients");
        var clients = await response.Content.ReadFromJsonAsync<IEnumerable<ClientDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        clients.Should().NotBeNull();
    }

    [Fact]
    public async Task GetClients_WithoutManagerRole_ShouldReturnForbidden()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "user@test.com",
            Email = "user@test.com"
        };
        await UserManager.CreateAsync(user, "Test123!");
        await UserManager.AddToRoleAsync(user, AppConst.Roles.Client);

        var loginDto = new LoginDto
        {
            Email = "user@test.com",
            Password = "Test123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Act
        var response = await Client.GetAsync("/api/clients");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateClient_WithValidData_ShouldCreateClient()
    {
        // Arrange
        await LoginAsManager();

        // Создаем клиента с уникальным email
        var uniqueEmail = $"client{DateTime.Now.Ticks}@test.com";
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = uniqueEmail,
            Phone = "+375291234567"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/clients", createClientDto);
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
            var client = JsonSerializer.Deserialize<ClientDto>(responseContent, options);
            client.Should().NotBeNull();
            client!.FirstName.Should().Be(createClientDto.FirstName);
            client.LastName.Should().Be(createClientDto.LastName);
            client.Email.Should().Be(createClientDto.Email);
            client.Phone.Should().Be(createClientDto.Phone);
        }
    }

    [Fact]
    public async Task CreateClient_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsManager();
        var createClientDto = new CreateClientDto
        {
            FirstName = "", // Пустое имя
            LastName = "User",
            Email = "invalid-email", // Неверный формат email
            Phone = "123" // Неверный формат телефона
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/clients", createClientDto);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateClient_WithDuplicatePhone_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsManager();
        var createClientDto1 = new CreateClientDto
        {
            FirstName = "Test1",
            LastName = "User1",
            Email = "test1@example.com",
            Phone = "+375291234567"
        };

        var createClientDto2 = new CreateClientDto
        {
            FirstName = "Test2",
            LastName = "User2",
            Email = "test2@example.com",
            Phone = "+375291234567" // Тот же телефон
        };

        // Act
        await Client.PostAsJsonAsync("/api/clients", createClientDto1);
        var response = await Client.PostAsJsonAsync("/api/clients", createClientDto2);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetClient_WithValidId_ShouldReturnClient()
    {
        // Arrange
        await LoginAsManager();
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Phone = "+375291234567"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/clients", createClientDto);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act
        var response = await Client.GetAsync($"/api/clients/{createdClient!.Id}");
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        client.Should().NotBeNull();
        client!.FirstName.Should().Be(createClientDto.FirstName);
        client.LastName.Should().Be(createClientDto.LastName);
        client.Email.Should().Be(createClientDto.Email);
        client.Phone.Should().Be(createClientDto.Phone);
    }

    [Fact]
    public async Task GetClient_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/clients/invalid-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetClientByPhone_WithValidPhone_ShouldReturnClient()
    {
        // Arrange
        await LoginAsManager();
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Phone = "+375291234567"
        };

        await Client.PostAsJsonAsync("/api/clients", createClientDto);

        // Act
        var response = await Client.GetAsync($"/api/clients/by-phone/{createClientDto.Phone}");
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        client.Should().NotBeNull();
        client!.Phone.Should().Be(createClientDto.Phone);
    }

    [Fact]
    public async Task GetClientByName_WithValidName_ShouldReturnClient()
    {
        // Arrange
        await LoginAsManager();
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Phone = "+375291234567"
        };

        await Client.PostAsJsonAsync("/api/clients", createClientDto);

        // Act
        var response =
            await Client.GetAsync(
                $"/api/clients/by-name?firstName={createClientDto.FirstName}&lastName={createClientDto.LastName}");
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        client.Should().NotBeNull();
        client!.FirstName.Should().Be(createClientDto.FirstName);
        client.LastName.Should().Be(createClientDto.LastName);
    }

    [Fact]
    public async Task UpdateClient_WithValidData_ShouldUpdateClient()
    {
        // Arrange
        await LoginAsManager();

        // Создаем клиента напрямую через сервис
        var uniqueEmail = $"test{DateTime.Now.Ticks}@example.com";
        var uniquePhone = $"+37529{DateTime.Now.Ticks % 1000000:D6}";

        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = uniqueEmail,
            Phone = uniquePhone
        };

        // Создаем клиента
        var createResponse = await Client.PostAsJsonAsync("/api/clients", createClientDto);
        var responseContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"Create Response Status: {createResponse.StatusCode}");
        _output.WriteLine($"Create Response Content: {responseContent}");

        if (createResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            _output.WriteLine($"Bad Request Details: {responseContent}");
            throw new Exception($"Failed to create client: {responseContent}");
        }

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, "Клиент должен быть успешно создан");

        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();
        _output.WriteLine($"Created Client ID: {createdClient!.Id}");

        var updateClientDto = new UpdateClientDto
        {
            FirstName = "Updated",
            LastName = "User",
            Email = $"updated{DateTime.Now.Ticks}@example.com",
            Phone = $"+37529{(DateTime.Now.Ticks + 1) % 1000000:D6}"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/clients/{createdClient.Id}", updateClientDto);
        _output.WriteLine($"Update Response Status: {response.StatusCode}");
        var updateResponseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Update Response Content: {updateResponseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "Обновление клиента должно вернуть 200 OK");

        var updatedClient = await response.Content.ReadFromJsonAsync<ClientDto>();
        _output.WriteLine($"Updated Client: {JsonSerializer.Serialize(updatedClient)}");

        updatedClient.Should().NotBeNull();
        updatedClient!.FirstName.Should().Be(updateClientDto.FirstName);
        updatedClient.LastName.Should().Be(updateClientDto.LastName);
        updatedClient.Email.Should().Be(updateClientDto.Email);
        updatedClient.Phone.Should().Be(updateClientDto.Phone);
    }

    [Fact]
    public async Task DeleteClient_WithValidId_ShouldDeleteClient()
    {
        // Arrange
        await LoginAsManager();

        // Создаем клиента напрямую через сервис
        var uniqueEmail = $"test{DateTime.Now.Ticks}@example.com";
        var uniquePhone = $"+37529{DateTime.Now.Ticks % 1000000:D6}";

        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = uniqueEmail,
            Phone = uniquePhone
        };

        // Создаем клиента
        var createResponse = await Client.PostAsJsonAsync("/api/clients", createClientDto);
        var responseContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"Create Response Status: {createResponse.StatusCode}");
        _output.WriteLine($"Create Response Content: {responseContent}");

        if (createResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            _output.WriteLine($"Bad Request Details: {responseContent}");
            throw new Exception($"Failed to create client: {responseContent}");
        }

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, "Клиент должен быть успешно создан");

        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();
        _output.WriteLine($"Created Client ID: {createdClient!.Id}");

        // Проверяем, что клиент существует
        var getResponse = await Client.GetAsync($"/api/clients/{createdClient.Id}");
        _output.WriteLine($"Get Response Status: {getResponse.StatusCode}");
        var getResponseContent = await getResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"Get Response Content: {getResponseContent}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK, "Клиент должен существовать перед удалением");

        // Act
        var response = await Client.DeleteAsync($"/api/clients/{createdClient.Id}");
        _output.WriteLine($"Delete Response Status: {response.StatusCode}");
        var deleteResponseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Delete Response Content: {deleteResponseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, "Удаление клиента должно вернуть 204 No Content");

        // Verify client is deleted
        var getDeletedResponse = await Client.GetAsync($"/api/clients/{createdClient.Id}");
        _output.WriteLine($"Get Deleted Response Status: {getDeletedResponse.StatusCode}");
        var getDeletedResponseContent = await getDeletedResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"Get Deleted Response Content: {getDeletedResponseContent}");

        // Проверяем, что ID в правильном формате
        _output.WriteLine($"Checking ID format: {createdClient.Id}");
        Guid.TryParse(createdClient.Id, out var _).Should().BeTrue("ID клиента должен быть в формате GUID");

        getDeletedResponse.StatusCode.Should()
            .Be(HttpStatusCode.NotFound, "После удаления клиент не должен существовать");
    }
}