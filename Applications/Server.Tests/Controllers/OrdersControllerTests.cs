using System.Security.Claims;
using Application.Controllers;
using Application.Data;
using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace Server.Tests.Controllers;

public class OrdersControllerTests : TestBase
{
    private readonly OrdersController _controller;
    private readonly Mock<IOrderService> _orderServiceMock;

    public OrdersControllerTests(ITestOutputHelper output) : base(output)
    {
        _orderServiceMock = new Mock<IOrderService>();
        _controller = new OrdersController(_orderServiceMock.Object);

        // Настройка базового пользователя с ролью менеджера
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, "test-user-id"),
            new(ClaimTypes.Role, "Manager")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    private OrderDto CreateTestOrderDto()
    {
        return new OrderDto
        {
            Id = 1,
            UserId = "test-user-id",
            UserName = "Test User",
            ClientName = "Test Client",
            ContactPhone = "1234567890",
            PaymentType = Order.PaymentTypes.Card,
            PaymentTypeDisplay = "Картой",
            OrderDate = DateTime.UtcNow,
            ChangeDate = DateTime.UtcNow,
            State = Order.States.New,
            StateDisplay = "В обработке",
            TotalAmount = 1000,
            Products = new List<OrderProductDto>(),
            Delivery = new DeliveryDto
            {
                OrderId = 1,
                DeliveryDate = DateTime.UtcNow.AddDays(1),
                City = "Test City",
                Street = "Test Street",
                House = "1",
                Flat = "1",
                PostalCode = "123456"
            }
        };
    }

    private CreateOrderDto CreateTestCreateOrderDto()
    {
        return new CreateOrderDto
        {
            StockId = 1,
            ClientName = "Test Client",
            ContactPhone = "1234567890",
            PaymentType = Order.PaymentTypes.Card,
            Products = new List<CreateOrderProductDto>
            {
                new()
                {
                    ProductId = 1,
                    Quantity = 2
                }
            },
            Delivery = new CreateDeliveryDto
            {
                DeliveryDate = DateTime.UtcNow.AddDays(1),
                City = "Test City",
                Street = "Test Street",
                House = "1",
                Flat = "1",
                PostalCode = "123456"
            }
        };
    }

    [Fact]
    public async Task GetOrders_AsManager_ShouldReturnOrders()
    {
        // Arrange
        await LoginAsManager();
        
        var expectedOrders = new List<OrderDto> { CreateTestOrderDto() };
        _orderServiceMock.Setup(x => x.GetAllOrdersAsync())
            .ReturnsAsync(expectedOrders);

        // Act
        var result = await _controller.GetOrders();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var orders = okResult.Value.Should().BeAssignableTo<IEnumerable<OrderDto>>().Subject;
        orders.Should().BeEquivalentTo(expectedOrders);
    }

    [Fact]
    public async Task GetMyOrders_AsUser_ShouldReturnUserOrders()
    {
        // Arrange
        await LoginAsClient();
        
        var expectedOrders = new List<OrderDto> { CreateTestOrderDto() };
        _orderServiceMock.Setup(x => x.GetOrdersByUserIdAsync("test-user-id"))
            .ReturnsAsync(expectedOrders);

        // Act
        var result = await _controller.GetMyOrders();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var orders = okResult.Value.Should().BeAssignableTo<IEnumerable<OrderDto>>().Subject;
        orders.Should().BeEquivalentTo(expectedOrders);
    }

    [Fact]
    public async Task GetOrder_AsManager_ShouldReturnAnyOrder()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 1;
        var expectedOrder = CreateTestOrderDto();
        _orderServiceMock.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var order = okResult.Value.Should().BeOfType<OrderDto>().Subject;
        order.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public async Task GetOrder_AsUser_ShouldReturnOwnOrder()
    {
        // Arrange
        await LoginAsClient();
        
        var orderId = 1;
        var expectedOrder = CreateTestOrderDto();
        _orderServiceMock.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync(expectedOrder);
        _orderServiceMock.Setup(x => x.IsOrderOwnerAsync(orderId, "test-user-id"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var order = okResult.Value.Should().BeOfType<OrderDto>().Subject;
        order.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public async Task GetOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 999;
        _orderServiceMock.Setup(x => x.GetOrderByIdAsync(orderId))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        await LoginAsClient();
        
        var createOrderDto = CreateTestCreateOrderDto();
        var expectedOrder = CreateTestOrderDto();
        _orderServiceMock.Setup(x => x.CreateOrderAsync(createOrderDto, "test-user-id"))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.CreateOrder(createOrderDto);

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var order = createdAtActionResult.Value.Should().BeOfType<OrderDto>().Subject;
        order.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsManager();
        
        var createOrderDto = new CreateOrderDto
        {
            ClientName = "",
            ContactPhone = "",
            Products = new List<CreateOrderProductDto>()
        };
        _controller.ModelState.AddModelError("ClientName", "Имя клиента обязательно для заполнения");

        // Act
        var result = await _controller.CreateOrder(createOrderDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateOrder_WithValidData_ShouldUpdateOrder()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 1;
        var updateOrderDto = new UpdateOrderDto
        {
            ClientName = "Updated Client",
            ContactPhone = "9876543210",
            PaymentType = Order.PaymentTypes.Cash,
            State = Order.States.InProcess
        };
        _orderServiceMock.Setup(x => x.IsOrderOwnerAsync(orderId, "test-user-id"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateOrder(orderId, updateOrderDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _orderServiceMock.Verify(x => x.UpdateOrderAsync(orderId, updateOrderDto), Times.Once);
    }

    [Fact]
    public async Task UpdateOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 999;
        var updateOrderDto = new UpdateOrderDto();
        _orderServiceMock.Setup(x => x.IsOrderOwnerAsync(orderId, "test-user-id"))
            .ReturnsAsync(true);
        _orderServiceMock.Setup(x => x.UpdateOrderAsync(orderId, updateOrderDto))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.UpdateOrder(orderId, updateOrderDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteOrder_WithValidId_ShouldDeleteOrder()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 1;
        _orderServiceMock.Setup(x => x.IsOrderOwnerAsync(orderId, "test-user-id"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteOrder(orderId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _orderServiceMock.Verify(x => x.DeleteOrderAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task DeleteOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = 999;
        _orderServiceMock.Setup(x => x.IsOrderOwnerAsync(orderId, "test-user-id"))
            .ReturnsAsync(true);
        _orderServiceMock.Setup(x => x.DeleteOrderAsync(orderId))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.DeleteOrder(orderId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Value.Should().Be($"Заказ с ID {orderId} не найден");
    }

    [Fact]
    public async Task UpdateDelivery_WithValidData_ShouldUpdateDelivery()
    {
        // Arrange
        var orderId = 1;
        var updateDeliveryDto = new UpdateDeliveryDto
        {
            DeliveryDate = DateTime.UtcNow.AddDays(2),
            City = "Updated City",
            Street = "Updated Street",
            House = "2",
            Flat = "2",
            PostalCode = "654321"
        };
        var expectedDelivery = new DeliveryDto
        {
            OrderId = orderId,
            DeliveryDate = updateDeliveryDto.DeliveryDate,
            City = updateDeliveryDto.City,
            Street = updateDeliveryDto.Street,
            House = updateDeliveryDto.House,
            Flat = updateDeliveryDto.Flat,
            PostalCode = updateDeliveryDto.PostalCode
        };
        _orderServiceMock.Setup(x => x.UpdateDeliveryAsync(orderId, updateDeliveryDto))
            .ReturnsAsync(expectedDelivery);

        // Act
        var result = await _controller.UpdateDelivery(orderId, updateDeliveryDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var delivery = okResult.Value.Should().BeOfType<DeliveryDto>().Subject;
        delivery.Should().BeEquivalentTo(expectedDelivery);
    }

    [Fact]
    public async Task UpdateDelivery_WithInvalidOrderId_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = 999;
        var updateDeliveryDto = new UpdateDeliveryDto();
        _orderServiceMock.Setup(x => x.UpdateDeliveryAsync(orderId, updateDeliveryDto))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.UpdateDelivery(orderId, updateDeliveryDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetDeliveriesByStatus_WithValidStatus_ShouldReturnDeliveries()
    {
        // Arrange
        var status = "В пути";
        var expectedDeliveries = new List<DeliveryDto>
        {
            new()
            {
                OrderId = 1,
                DeliveryDate = DateTime.UtcNow.AddDays(1),
                City = "Test City",
                Street = "Test Street",
                House = "1",
                Flat = "1",
                PostalCode = "123456"
            }
        };
        _orderServiceMock.Setup(x => x.GetDeliveriesByStatusAsync(status))
            .ReturnsAsync(expectedDeliveries);

        // Act
        var result = await _controller.GetDeliveriesByStatus(status);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var deliveries = okResult.Value.Should().BeAssignableTo<IEnumerable<DeliveryDto>>().Subject;
        deliveries.Should().BeEquivalentTo(expectedDeliveries);
    }

    [Fact]
    public async Task GetDeliveriesByDateRange_WithValidDates_ShouldReturnDeliveries()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(7);
        var expectedDeliveries = new List<DeliveryDto>
        {
            new()
            {
                OrderId = 1,
                DeliveryDate = DateTime.UtcNow.AddDays(1),
                City = "Test City",
                Street = "Test Street",
                House = "1",
                Flat = "1",
                PostalCode = "123456"
            }
        };
        _orderServiceMock.Setup(x => x.GetDeliveriesByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(expectedDeliveries);

        // Act
        var result = await _controller.GetDeliveriesByDateRange(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var deliveries = okResult.Value.Should().BeAssignableTo<IEnumerable<DeliveryDto>>().Subject;
        deliveries.Should().BeEquivalentTo(expectedDeliveries);
    }

    [Fact]
    public async Task UpdateOrder_WithoutManagerRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsManager();

        // Act
        var result = await _controller.UpdateOrder(1, new UpdateOrderDto());

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task UpdateOrder_WithInvalidState_ShouldReturnBadRequest()
    {
        // Arrange
        var orderId = 1;
        var updateOrderDto = new UpdateOrderDto
        {
            State = Order.States.Completed
        };
        _orderServiceMock.Setup(x => x.IsOrderOwnerAsync(orderId, "test-user-id"))
            .ReturnsAsync(true);
        _orderServiceMock.Setup(x => x.UpdateOrderAsync(orderId, updateOrderDto))
            .ThrowsAsync(new InvalidOrderStateException("Нельзя изменить статус заказа"));

        // Act
        var result = await _controller.UpdateOrder(orderId, updateOrderDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrderDto = CreateTestCreateOrderDto();
        var productId = createOrderDto.Products.First().ProductId;
        var requestedQuantity = createOrderDto.Products.First().Quantity;
        var availableQuantity = 1;

        _orderServiceMock.Setup(x => x.CreateOrderAsync(createOrderDto, "test-user-id"))
            .ThrowsAsync(new InsufficientStockException(productId, requestedQuantity, availableQuantity));

        // Act
        var result = await _controller.CreateOrder(createOrderDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Value.Should()
            .Be($"Недостаточно товара с ID {productId}. Запрошено: {requestedQuantity}, Доступно: {availableQuantity}");
    }

    [Fact]
    public async Task UpdateDelivery_WithInvalidState_ShouldReturnBadRequest()
    {
        // Arrange
        var orderId = 1;
        var updateDeliveryDto = new UpdateDeliveryDto();
        _orderServiceMock.Setup(x => x.UpdateDeliveryAsync(orderId, updateDeliveryDto))
            .ThrowsAsync(new InvalidOrderStateException("Нельзя изменить доставку отмененного заказа"));

        // Act
        var result = await _controller.UpdateDelivery(orderId, updateDeliveryDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExecuteOrder_WithValidId_ShouldExecuteOrder()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 1;
        var expectedOrder = CreateTestOrderDto();
        expectedOrder.State = Order.States.InProcess;
        _orderServiceMock.Setup(x => x.ExecuteOrderAsync(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.ExecuteOrder(orderId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var order = okResult.Value.Should().BeOfType<OrderDto>().Subject;
        order.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public async Task ExecuteOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = 999;
        _orderServiceMock.Setup(x => x.ExecuteOrderAsync(orderId))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.ExecuteOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ExecuteOrder_WithoutStock_ShouldReturnBadRequest()
    {
        // Arrange
        var orderId = 1;
        _orderServiceMock.Setup(x => x.ExecuteOrderAsync(orderId))
            .ThrowsAsync(new StockNotFoundException(0));

        // Act
        var result = await _controller.ExecuteOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExecuteOrder_WithInsufficientStock_ShouldReturnBadRequest()
    {
        // Arrange
        var orderId = 1;
        var productId = 1;
        var requestedQuantity = 10;
        var availableQuantity = 5;
        _orderServiceMock.Setup(x => x.ExecuteOrderAsync(orderId))
            .ThrowsAsync(new InsufficientStockException(productId, requestedQuantity, availableQuantity));

        // Act
        var result = await _controller.ExecuteOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Fact]
    public async Task CancelOrder_WithValidId_ShouldCancelOrder()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 1;
        var expectedOrder = CreateTestOrderDto();
        expectedOrder.State = Order.States.Cancelled;
        _orderServiceMock.Setup(x => x.CancelOrderAsync(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.CancelOrder(orderId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var order = okResult.Value.Should().BeOfType<OrderDto>().Subject;
        order.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public async Task CancelOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = 999;
        _orderServiceMock.Setup(x => x.CancelOrderAsync(orderId))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.CancelOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CancelOrder_WithCompletedOrder_ShouldReturnBadRequest()
    {
        // Arrange
        var orderId = 1;
        _orderServiceMock.Setup(x => x.CancelOrderAsync(orderId))
            .ThrowsAsync(new InvalidOrderStateException("Нельзя отменить завершенный заказ"));

        // Act
        var result = await _controller.CancelOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CancelOrder_WithInProcessOrder_ShouldReturnOk()
    {
        // Arrange
        var orderId = 1;
        var expectedOrder = CreateTestOrderDto();
        expectedOrder.State = Order.States.Cancelled;
        _orderServiceMock.Setup(x => x.CancelOrderAsync(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.CancelOrder(orderId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var order = okResult.Value.Should().BeOfType<OrderDto>().Subject;
        order.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public async Task CompleteOrder_WithValidId_ShouldCompleteOrder()
    {
        // Arrange
        await LoginAsManager();
        
        var orderId = 1;
        var expectedOrder = CreateTestOrderDto();
        expectedOrder.State = Order.States.Completed;
        _orderServiceMock.Setup(x => x.CompleteOrderAsync(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.CompleteOrder(orderId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var order = okResult.Value.Should().BeOfType<OrderDto>().Subject;
        order.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public async Task CompleteOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = 999;
        _orderServiceMock.Setup(x => x.CompleteOrderAsync(orderId))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.CompleteOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CompleteOrder_WithCompletedOrder_ShouldReturnBadRequest()
    {
        // Arrange
        var orderId = 1;
        _orderServiceMock.Setup(x => x.CompleteOrderAsync(orderId))
            .ThrowsAsync(new InvalidOrderStateException("Заказ уже завершен"));

        // Act
        var result = await _controller.CompleteOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}