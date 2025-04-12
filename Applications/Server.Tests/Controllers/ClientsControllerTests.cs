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
        await LoginAsClient();

        // Act
        var response = await Client.GetAsync("/api/clients");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ExistsForCurrentUser_AsClient_ShouldReturnTrue()
    {
        // Arrange
        await LoginAsClient();
        
        // Создаем клиента для текущего пользователя
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Phone = $"+37529{DateTime.Now.Ticks % 1000000:D6}"
        };
        await Client.PostAsJsonAsync("/api/clients/create-for-current", createClientDto);

        // Act
        var response = await Client.GetAsync("/api/clients/exists");
        var exists = await response.Content.ReadFromJsonAsync<bool>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task CreateForCurrentUser_AsClient_ShouldCreateClient()
    {
        // Arrange
        await LoginAsClient();
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Phone = "+375291234567"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/clients/create-for-current", createClientDto);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        client.Should().NotBeNull();
        client!.FirstName.Should().Be(createClientDto.FirstName);
        client.LastName.Should().Be(createClientDto.LastName);
        client.Phone.Should().Be(createClientDto.Phone);
    }

    [Fact]
    public async Task CreateForCurrentUser_WithDuplicatePhone_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsClient();
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Phone = "+375291234567"
        };

        // Act
        await Client.PostAsJsonAsync("/api/clients/create-for-current", createClientDto);
        var response = await Client.PostAsJsonAsync("/api/clients/create-for-current", createClientDto);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateFullUser_AsManager_ShouldCreateUserAndClient()
    {
        // Arrange
        await LoginAsManager();
        var createFullUserDto = new CreateFullUserDto
        {
            Email = $"test{DateTime.Now.Ticks}@example.com",
            FirstName = "Test",
            LastName = "User",
            Phone = $"+37529{DateTime.Now.Ticks % 1000000:D6}"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/clients/create-full", createFullUserDto);
        var result = await response.Content.ReadFromJsonAsync<CreateFullUserResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Email.Should().Be(createFullUserDto.Email);
        result.DefaultPassword.Should().Be(AppConst.Password.DefaultPassword);
        result.Client.Should().NotBeNull();
        result.Client.FirstName.Should().Be(createFullUserDto.FirstName);
        result.Client.LastName.Should().Be(createFullUserDto.LastName);
        result.Client.Phone.Should().Be(createFullUserDto.Phone);
    }

    [Fact]
    public async Task CreateFullUser_WithoutManagerRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsClient();
        var createFullUserDto = new CreateFullUserDto
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Phone = "+375291234567"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/clients/create-full", createFullUserDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateSelf_AsClient_ShouldUpdateClient()
    {
        // Arrange
        await LoginAsClient();
        
        // Создаем клиента для текущего пользователя
        var createClientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Phone = $"+37529{DateTime.Now.Ticks % 1000000:D6}"
        };
        await Client.PostAsJsonAsync("/api/clients/create-for-current", createClientDto);

        // Подготавливаем данные для обновления
        var updateClientDto = new UpdateClientDto
        {
            FirstName = "Updated",
            LastName = "User",
            Phone = $"+37529{(DateTime.Now.Ticks + 1) % 1000000:D6}"
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/clients/update-self", updateClientDto);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        client.Should().NotBeNull();
        client!.FirstName.Should().Be(updateClientDto.FirstName);
        client.LastName.Should().Be(updateClientDto.LastName);
        client.Phone.Should().Be(updateClientDto.Phone);
    }

    [Fact]
    public async Task UpdateSelf_WithDuplicatePhone_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsClient();
        var phone = $"+37529{DateTime.Now.Ticks % 1000000:D6}";
        
        // Создаем второго клиента с тем же телефоном
        await LoginAsManager();
        var createFullUserDto = new CreateFullUserDto
        {
            Email = $"test{DateTime.Now.Ticks}@example.com",
            FirstName = "Test",
            LastName = "User",
            Phone = phone
        };
        await Client.PostAsJsonAsync("/api/clients/create-full", createFullUserDto);

        // Пытаемся обновить телефон первого клиента
        await LoginAsClient();
        var updateClientDto = new UpdateClientDto
        {
            FirstName = "Updated",
            LastName = "User",
            Phone = phone
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/clients/update-self", updateClientDto);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetClient_AsManager_ShouldReturnClient()
    {
        // Arrange
        await LoginAsManager();
        var createFullUserDto = new CreateFullUserDto
        {
            Email = $"test{DateTime.Now.Ticks}@example.com",
            FirstName = "Test",
            LastName = "User",
            Phone = $"+37529{DateTime.Now.Ticks % 1000000:D6}"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/clients/create-full", createFullUserDto);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<CreateFullUserResponseDto>();

        // Act
        var response = await Client.GetAsync($"/api/clients/{createdUser!.Client.Id}");
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        client.Should().NotBeNull();
        client!.FirstName.Should().Be(createFullUserDto.FirstName);
        client.LastName.Should().Be(createFullUserDto.LastName);
        client.Phone.Should().Be(createFullUserDto.Phone);
    }

    [Fact]
    public async Task GetClient_WithoutManagerRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsClient();

        // Act
        var response = await Client.GetAsync("/api/clients/some-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteClient_AsManager_ShouldDeleteClient()
    {
        // Arrange
        await LoginAsManager();
        var createFullUserDto = new CreateFullUserDto
        {
            Email = $"test{DateTime.Now.Ticks}@example.com",
            FirstName = "Test",
            LastName = "User",
            Phone = $"+37529{DateTime.Now.Ticks % 1000000:D6}"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/clients/create-full", createFullUserDto);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<CreateFullUserResponseDto>();

        // Act
        var response = await Client.DeleteAsync($"/api/clients/{createdUser!.Client.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify client is deleted
        var getResponse = await Client.GetAsync($"/api/clients/{createdUser.Client.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteClient_WithoutManagerRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsClient();

        // Act
        var response = await Client.DeleteAsync("/api/clients/some-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}