using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using Application.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Server.Tests.Controllers
{
    public class ClientsControllerTests : TestBase
    {
        private readonly string _managerToken;
        private readonly string _userToken;

        public ClientsControllerTests()
        {
            // Создаем менеджера для тестов
            var manager = new AppUser
            {
                UserName = "manager@test.com",
                Email = "manager@test.com"
            };
            UserManager.CreateAsync(manager, "Test123!").Wait();
            UserManager.AddToRoleAsync(manager, "Manager").Wait();

            // Создаем обычного пользователя для тестов
            var user = new AppUser
            {
                UserName = "user@test.com",
                Email = "user@test.com"
            };
            UserManager.CreateAsync(user, "Test123!").Wait();
            UserManager.AddToRoleAsync(user, "User").Wait();

            // Получаем токены
            var managerLoginResponse = Client.PostAsJsonAsync("/api/auth/login", new LoginDto
            {
                Email = "manager@test.com",
                Password = "Test123!"
            }).Result;
            var managerLoginResult = managerLoginResponse.Content.ReadFromJsonAsync<AuthResponseDto>().Result;
            _managerToken = managerLoginResult!.Token;

            var userLoginResponse = Client.PostAsJsonAsync("/api/auth/login", new LoginDto
            {
                Email = "user@test.com",
                Password = "Test123!"
            }).Result;
            var userLoginResult = userLoginResponse.Content.ReadFromJsonAsync<AuthResponseDto>().Result;
            _userToken = userLoginResult!.Token;
        }

        [Fact]
        public async Task GetClients_WithManagerRole_ShouldReturnClients()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_managerToken}");

            // Act
            var response = await Client.GetAsync("/api/clients");
            var clients = await response.Content.ReadFromJsonAsync<IEnumerable<ClientDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            clients.Should().NotBeNull();
        }

        [Fact]
        public async Task GetClients_WithUserRole_ShouldReturnForbidden()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userToken}");

            // Act
            var response = await Client.GetAsync("/api/clients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CreateClient_WithValidData_ShouldCreateClient()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_managerToken}");
            var createClientDto = new CreateClientDto
            {
                FirstName = "Test",
                LastName = "User",
                Phone = "+375291234567",
                Email = "testclient@example.com"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/clients", createClientDto);
            var client = await response.Content.ReadFromJsonAsync<ClientDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            client.Should().NotBeNull();
            client!.FirstName.Should().Be(createClientDto.FirstName);
            client.LastName.Should().Be(createClientDto.LastName);
            client.Phone.Should().Be(createClientDto.Phone);
            client.Email.Should().Be(createClientDto.Email);
        }

        [Fact]
        public async Task CreateClient_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_managerToken}");
            var createClientDto = new CreateClientDto
            {
                FirstName = "",  // Пустое имя
                LastName = "User",
                Phone = "+375291234567",
                Email = "testclient@example.com"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/clients", createClientDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetClientByPhone_WithExistingPhone_ShouldReturnClient()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_managerToken}");
            var createClientDto = new CreateClientDto
            {
                FirstName = "Test",
                LastName = "User",
                Phone = "+375291234567",
                Email = "testclient2@example.com"
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
        public async Task GetClientByPhone_WithNonExistingPhone_ShouldReturnNotFound()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_managerToken}");

            // Act
            var response = await Client.GetAsync("/api/clients/by-phone/+375299999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateClient_WithValidData_ShouldUpdateClient()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_managerToken}");
            var createClientDto = new CreateClientDto
            {
                FirstName = "Test",
                LastName = "User",
                Phone = "+375291234567",
                Email = "testclient3@example.com"
            };
            var createResponse = await Client.PostAsJsonAsync("/api/clients", createClientDto);
            var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

            var updateClientDto = new UpdateClientDto
            {
                FirstName = "Updated",
                LastName = "User",
                Phone = "+375291234567",
                Email = "updated@example.com"
            };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/clients/{createdClient!.Id}", updateClientDto);
            var updatedClient = await response.Content.ReadFromJsonAsync<ClientDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedClient.Should().NotBeNull();
            updatedClient!.FirstName.Should().Be(updateClientDto.FirstName);
            updatedClient.Email.Should().Be(updateClientDto.Email);
        }

        [Fact]
        public async Task DeleteClient_WithExistingId_ShouldDeleteClient()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_managerToken}");
            var createClientDto = new CreateClientDto
            {
                FirstName = "Test",
                LastName = "User",
                Phone = "+375291234567",
                Email = "testclient4@example.com"
            };
            var createResponse = await Client.PostAsJsonAsync("/api/clients", createClientDto);
            var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

            // Act
            var response = await Client.DeleteAsync($"/api/clients/{createdClient!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify client is deleted
            var getResponse = await Client.GetAsync($"/api/clients/{createdClient.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
} 