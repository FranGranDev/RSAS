using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit.Abstractions;

namespace Server.Tests.Controllers;

public class ProductsControllerTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public async Task GetProducts_AsManager_ShouldReturnProducts()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/products");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducts_WithoutManagerRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsClient();

        // Act
        var response = await Client.GetAsync("/api/products");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = $"Test Product {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description",
            Barcode = $"TEST{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be(createProductDto.Name);
        createdProduct.Price.Should().Be(createProductDto.Price);
        createdProduct.Description.Should().Be(createProductDto.Description);
        createdProduct.Barcode.Should().Be(createProductDto.Barcode);
        createdProduct.Category.Should().Be(createProductDto.Category);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = "", // Невалидное имя
            Price = -1, // Невалидная цена
            Description = "Test Description",
            Barcode = "", // Невалидный штрих-код
            Category = "" // Невалидная категория
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = $"Test Product {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description",
            Barcode = $"TEST{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        // Создаем первый товар
        await Client.PostAsJsonAsync("/api/products", createProductDto);

        // Act
        // Пытаемся создать второй товар с тем же именем
        var response = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = $"Test Product {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description",
            Barcode = $"TEST{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await Client.GetAsync($"/api/products/{createdProduct!.Id}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be(createdProduct.Name);
        product.Price.Should().Be(createdProduct.Price);
        product.Description.Should().Be(createdProduct.Description);
        product.Barcode.Should().Be(createdProduct.Barcode);
        product.Category.Should().Be(createdProduct.Category);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var response = await Client.GetAsync("/api/products/999999");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = $"Test Product {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description",
            Barcode = $"TEST{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateProductDto = new UpdateProductDto
        {
            Name = $"Updated Product {DateTime.Now.Ticks}",
            Price = 200.00m,
            Description = "Updated Description",
            Barcode = $"UPDATED{DateTime.Now.Ticks}",
            Category = "Updated Category"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/products/{createdProduct!.Id}", updateProductDto);
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Id.Should().Be(createdProduct.Id);
        updatedProduct.Name.Should().Be(updateProductDto.Name);
        updatedProduct.Price.Should().Be(updateProductDto.Price);
        updatedProduct.Description.Should().Be(updateProductDto.Description);
        updatedProduct.Barcode.Should().Be(updateProductDto.Barcode);
        updatedProduct.Category.Should().Be(updateProductDto.Category);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ShouldDeleteProduct()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = $"Test Product {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description",
            Barcode = $"TEST{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await Client.DeleteAsync($"/api/products/{createdProduct!.Id}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify product is deleted
        var getResponse = await Client.GetAsync($"/api/products/{createdProduct.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProductByName_WithValidName_ShouldReturnProduct()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = $"Test Product {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description",
            Barcode = $"TEST{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await Client.GetAsync($"/api/products/name/{createdProduct!.Name}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be(createdProduct.Name);
        product.Price.Should().Be(createdProduct.Price);
        product.Description.Should().Be(createdProduct.Description);
        product.Barcode.Should().Be(createdProduct.Barcode);
        product.Category.Should().Be(createdProduct.Category);
    }

    [Fact]
    public async Task GetProductByBarcode_WithValidBarcode_ShouldReturnProduct()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto = new CreateProductDto
        {
            Name = $"Test Product {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description",
            Barcode = $"TEST{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await Client.GetAsync($"/api/products/barcode/{createdProduct!.Barcode}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be(createdProduct.Name);
        product.Price.Should().Be(createdProduct.Price);
        product.Description.Should().Be(createdProduct.Description);
        product.Barcode.Should().Be(createdProduct.Barcode);
        product.Category.Should().Be(createdProduct.Category);
    }

    [Fact]
    public async Task GetProductsByCategory_WithValidCategory_ShouldReturnProducts()
    {
        // Arrange
        await LoginAsManager();

        var category = $"Test Category {DateTime.Now.Ticks}";

        var createProductDto1 = new CreateProductDto
        {
            Name = $"Test Product 1 {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description 1",
            Barcode = $"TEST1{DateTime.Now.Ticks}",
            Category = category
        };

        var createProductDto2 = new CreateProductDto
        {
            Name = $"Test Product 2 {DateTime.Now.Ticks}",
            Price = 200.00m,
            Description = "Test Description 2",
            Barcode = $"TEST2{DateTime.Now.Ticks}",
            Category = category
        };

        await Client.PostAsJsonAsync("/api/products", createProductDto1);
        await Client.PostAsJsonAsync("/api/products", createProductDto2);

        // Act
        var response = await Client.GetAsync($"/api/products/category/{category}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().HaveCount(2);
        products.Should().OnlyContain(p => p.Category == category);
    }

    [Fact]
    public async Task GetProductsByPriceRange_WithValidRange_ShouldReturnProducts()
    {
        // Arrange
        await LoginAsManager();

        var createProductDto1 = new CreateProductDto
        {
            Name = $"Test Product 1 {DateTime.Now.Ticks}",
            Price = 100.00m,
            Description = "Test Description 1",
            Barcode = $"TEST1{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        var createProductDto2 = new CreateProductDto
        {
            Name = $"Test Product 2 {DateTime.Now.Ticks}",
            Price = 200.00m,
            Description = "Test Description 2",
            Barcode = $"TEST2{DateTime.Now.Ticks}",
            Category = "Test Category"
        };

        await Client.PostAsJsonAsync("/api/products", createProductDto1);
        await Client.PostAsJsonAsync("/api/products", createProductDto2);

        // Act
        var response = await Client.GetAsync("/api/products/price-range?minPrice=50&maxPrice=150");
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
        products.Should().OnlyContain(p => p.Price >= 50 && p.Price <= 150);
    }
}