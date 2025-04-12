using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Data;
using Application.DTOs;
using Application.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Server.Tests.Controllers;

public class StocksControllerTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public async Task GetStocks_AsManager_ShouldReturnStocks()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/stocks");
        var stocks = await response.Content.ReadFromJsonAsync<IEnumerable<StockDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stocks.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStocks_WithoutManagerRole_ShouldReturnForbidden()
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
        var response = await Client.GetAsync("/api/stocks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateStock_WithValidData_ShouldCreateStock()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/stocks", createStockDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var stock = await response.Content.ReadFromJsonAsync<StockDto>();
        stock.Should().NotBeNull();
        stock!.Name.Should().Be(createStockDto.Name);
        stock.Address.Should().Be(createStockDto.Address);
        stock.City.Should().Be(createStockDto.City);
        stock.Phone.Should().Be(createStockDto.Phone);
        stock.Email.Should().Be(createStockDto.Email);
    }

    [Fact]
    public async Task CreateStock_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = "", // Пустое название
            Address = "Test Address",
            City = "Test City",
            Phone = "123", // Неверный формат телефона
            Email = "invalid-email" // Неверный формат email
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/stocks", createStockDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateStock_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto1 = new CreateStockDto
        {
            Name = "Test Stock",
            Address = "Test Address 1",
            City = "Test City",
            Phone = "+375291234567",
            Email = "test1@example.com"
        };

        var createStockDto2 = new CreateStockDto
        {
            Name = "Test Stock", // То же название
            Address = "Test Address 2",
            City = "Test City",
            Phone = "+375297654321",
            Email = "test2@example.com"
        };

        // Act
        await Client.PostAsJsonAsync("/api/stocks", createStockDto1);
        var response = await Client.PostAsJsonAsync("/api/stocks", createStockDto2);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetStock_WithValidId_ShouldReturnStock()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/stocks", createStockDto);
        var createdStock = await createResponse.Content.ReadFromJsonAsync<StockDto>();

        // Act
        var response = await Client.GetAsync($"/api/stocks/{createdStock!.Id}");
        var stock = await response.Content.ReadFromJsonAsync<StockDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stock.Should().NotBeNull();
        stock!.Name.Should().Be(createStockDto.Name);
        stock.Address.Should().Be(createStockDto.Address);
        stock.City.Should().Be(createStockDto.City);
        stock.Phone.Should().Be(createStockDto.Phone);
        stock.Email.Should().Be(createStockDto.Email);
    }

    [Fact]
    public async Task GetStock_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/stocks/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateStock_WithValidData_ShouldUpdateStock()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/stocks", createStockDto);
        var createdStock = await createResponse.Content.ReadFromJsonAsync<StockDto>();

        var updateStockDto = new UpdateStockDto
        {
            Name = $"Updated Stock {DateTime.Now.Ticks}",
            Address = $"Updated Address {DateTime.Now.Ticks}",
            City = "Updated City",
            Phone = "+375297654321",
            Email = $"updated{DateTime.Now.Ticks}@example.com"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/stocks/{createdStock!.Id}", updateStockDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedStock = await response.Content.ReadFromJsonAsync<StockDto>();
        updatedStock.Should().NotBeNull();
        updatedStock!.Name.Should().Be(updateStockDto.Name);
        updatedStock.Address.Should().Be(updateStockDto.Address);
        updatedStock.City.Should().Be(updateStockDto.City);
        updatedStock.Phone.Should().Be(updateStockDto.Phone);
        updatedStock.Email.Should().Be(updateStockDto.Email);
    }

    [Fact]
    public async Task DeleteStock_WithValidId_ShouldDeleteStock()
    {
        // Arrange
        await LoginAsManager();

        // Создаем склад
        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/stocks", createStockDto);
        var createdStock = await createResponse.Content.ReadFromJsonAsync<StockDto>();

        // Act
        var response = await Client.DeleteAsync($"/api/stocks/{createdStock!.Id}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify stock is deleted
        var getResponse = await Client.GetAsync($"/api/stocks/{createdStock.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStockByName_WithValidName_ShouldReturnStock()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/stocks", createStockDto);
        var createdStock = await createResponse.Content.ReadFromJsonAsync<StockDto>();

        // Act
        var response = await Client.GetAsync($"/api/stocks/name/{createdStock!.Name}");
        var stock = await response.Content.ReadFromJsonAsync<StockDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stock.Should().NotBeNull();
        stock!.Name.Should().Be(createStockDto.Name);
    }

    [Fact]
    public async Task GetStockByAddress_WithValidAddress_ShouldReturnStock()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/stocks", createStockDto);
        var createdStock = await createResponse.Content.ReadFromJsonAsync<StockDto>();

        // Act
        var response = await Client.GetAsync($"/api/stocks/address/{createdStock!.Address}");
        var stock = await response.Content.ReadFromJsonAsync<StockDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stock.Should().NotBeNull();
        stock!.Address.Should().Be(createStockDto.Address);
    }

    [Fact]
    public async Task GetStocksByCity_WithValidCity_ShouldReturnStocks()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        await Client.PostAsJsonAsync("/api/stocks", createStockDto);

        // Act
        var response = await Client.GetAsync($"/api/stocks/city/{createStockDto.City}");
        var stocks = await response.Content.ReadFromJsonAsync<IEnumerable<StockDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stocks.Should().NotBeNull();
        stocks.Should().Contain(s => s.City == createStockDto.City);
    }

    [Fact]
    public async Task ExistsByName_WithValidName_ShouldReturnTrue()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        await Client.PostAsJsonAsync("/api/stocks", createStockDto);

        // Act
        var response = await Client.GetAsync($"/api/stocks/exists/name/{createStockDto.Name}");
        var exists = await response.Content.ReadFromJsonAsync<bool>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByAddress_WithValidAddress_ShouldReturnTrue()
    {
        // Arrange
        await LoginAsManager();

        var createStockDto = new CreateStockDto
        {
            Name = $"Test Stock {DateTime.Now.Ticks}",
            Address = $"Test Address {DateTime.Now.Ticks}",
            City = "Test City",
            Phone = "+375291234567",
            Email = $"test{DateTime.Now.Ticks}@example.com"
        };

        await Client.PostAsJsonAsync("/api/stocks", createStockDto);

        // Act
        var response = await Client.GetAsync($"/api/stocks/exists/address/{createStockDto.Address}");
        var exists = await response.Content.ReadFromJsonAsync<bool>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        exists.Should().BeTrue();
    }
}