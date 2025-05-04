using System.Security.Claims;
using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Server.Controllers;
using Server.Services.Sales;
using Xunit.Abstractions;

namespace Server.Tests.Controllers;

public class SalesControllerTests : TestBase
{
    private readonly Mock<ISaleService> _saleServiceMock;
    private readonly SalesController _controller;
    private readonly ITestOutputHelper _output;
    private readonly Random _random;
    private readonly List<string> _productCategories;
    private readonly List<string> _productNames;
    private readonly List<string> _stockNames;
    private readonly List<string> _clientNames;

    public SalesControllerTests(ITestOutputHelper output) : base(output)
    {
        _output = output;
        _saleServiceMock = new Mock<ISaleService>();
        _controller = new SalesController(_saleServiceMock.Object);
        _random = new Random(42); // Фиксированный seed для воспроизводимости

        // Инициализация тестовых данных
        _productCategories = new List<string>
        {
            "Электроника", "Одежда", "Продукты", "Книги", "Спорттовары",
            "Косметика", "Мебель", "Игрушки", "Автозапчасти", "Строительные материалы"
        };

        _productNames = new List<string>
        {
            "Смартфон", "Ноутбук", "Планшет", "Телевизор", "Наушники",
            "Футболка", "Джинсы", "Куртка", "Обувь", "Аксессуары",
            "Хлеб", "Молоко", "Мясо", "Овощи", "Фрукты",
            "Роман", "Детектив", "Фантастика", "Учебник", "Энциклопедия"
        };

        _stockNames = new List<string>
        {
            "Центральный склад", "Северный склад", "Южный склад",
            "Западный склад", "Восточный склад", "Главный склад"
        };

        _clientNames = new List<string>
        {
            "Иванов Иван", "Петров Петр", "Сидоров Сидор",
            "Смирнова Анна", "Козлова Елена", "Николаев Николай"
        };

        // Настройка пользователя с ролью менеджера
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, "test-user-id"),
            new(ClaimTypes.Name, "Test User"),
            new(ClaimTypes.Role, "Manager")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    private SaleDto CreateRandomSaleDto(int id)
    {
        var productsCount = _random.Next(1, 10);
        var products = new List<SaleProductDto>();
        var totalAmount = 0m;

        for (int i = 0; i < productsCount; i++)
        {
            var quantity = _random.Next(1, 5);
            var price = _random.Next(100, 10000);
            var discount = _random.Next(0, 30);
            var discountAmount = price * quantity * discount / 100;
            var productAmount = price * quantity - discountAmount;

            products.Add(new SaleProductDto
            {
                Id = i + 1,
                SaleId = id,
                ProductId = _random.Next(1, 1000),
                ProductName = _productNames[_random.Next(_productNames.Count)],
                ProductCategory = _productCategories[_random.Next(_productCategories.Count)],
                Quantity = quantity,
                ProductPrice = price,
                DiscountAmount = discountAmount
            });

            totalAmount += productAmount;
        }

        return new SaleDto
        {
            Id = id,
            OrderId = id,
            OrderNumber = $"ORD-{id:D6}",
            StockId = _random.Next(1, 6),
            StockName = _stockNames[_random.Next(_stockNames.Count)],
            SaleDate = DateTime.UtcNow.AddDays(-_random.Next(0, 365)),
            TotalAmount = totalAmount,
            ClientName = _clientNames[_random.Next(_clientNames.Count)],
            ClientPhone = $"+7{_random.Next(900, 999)}{_random.Next(1000000, 9999999)}",
            Products = products
        };
    }

    private List<SaleDto> GenerateLargeSalesDataset()
    {
        var count = _random.Next(500, 5001);
        var sales = new List<SaleDto>();
        for (int i = 1; i <= count; i++)
        {
            sales.Add(CreateRandomSaleDto(i));
        }
        return sales;
    }

    private DashboardAnalyticsDto CreateRealisticDashboardAnalytics(List<SaleDto> sales)
    {
        var totalRevenue = sales.Sum(s => s.TotalAmount);
        var totalSalesCount = sales.Count;
        var totalOrdersCount = sales.Select(s => s.OrderId).Distinct().Count();
        var averageOrderAmount = totalRevenue / totalSalesCount;

        var topProducts = sales
            .SelectMany(s => s.Products)
            .GroupBy(p => p.ProductName)
            .Select(g => new TopProductResultDto
            {
                ProductName = g.Key,
                SalesCount = g.Sum(p => p.Quantity),
                Revenue = g.Sum(p => p.ProductPrice * p.Quantity - p.DiscountAmount)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(10)
            .ToList();

        var orderStatusStats = new OrderStatusStatsDto
        {
            NewCount = _random.Next(50, 100),
            ProcessingCount = _random.Next(30, 80),
            CompletedCount = _random.Next(200, 400),
            CancelledCount = _random.Next(20, 50)
        };

        return new DashboardAnalyticsDto
        {
            TotalRevenue = totalRevenue,
            TotalSalesCount = totalSalesCount,
            AverageOrderAmount = averageOrderAmount,
            TotalOrdersCount = totalOrdersCount,
            TopProducts = topProducts,
            OrderStatusStats = orderStatusStats
        };
    }

    private SalesAnalyticsDto CreateRealisticSalesAnalytics(List<SaleDto> sales, DateTime startDate, DateTime endDate)
    {
        var periodSales = sales.Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate).ToList();
        var totalRevenue = periodSales.Sum(s => s.TotalAmount);
        var totalSalesCount = periodSales.Count;
        var averageSaleAmount = totalSalesCount > 0 ? totalRevenue / totalSalesCount : 0;

        var categorySales = periodSales
            .SelectMany(s => s.Products)
            .GroupBy(p => p.ProductCategory)
            .Select(g => new CategorySalesResultDto
            {
                Category = g.Key,
                SalesCount = g.Sum(p => p.Quantity),
                Revenue = g.Sum(p => p.ProductPrice * p.Quantity - p.DiscountAmount),
                Share = totalRevenue > 0 ? g.Sum(p => p.ProductPrice * p.Quantity - p.DiscountAmount) / totalRevenue : 0
            })
            .OrderByDescending(c => c.Revenue)
            .ToList();

        var salesTrend = new List<SalesTrendResultDto>();
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            var dailySales = periodSales.Where(s => s.SaleDate.Date == currentDate.Date).ToList();
            salesTrend.Add(new SalesTrendResultDto
            {
                Date = currentDate,
                Revenue = dailySales.Sum(s => s.TotalAmount),
                SalesCount = dailySales.Count
            });
            currentDate = currentDate.AddDays(1);
        }

        return new SalesAnalyticsDto
        {
            Period = $"{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}",
            Revenue = totalRevenue,
            SalesCount = totalSalesCount,
            AverageSaleAmount = averageSaleAmount,
            CategorySales = categorySales,
            SalesTrend = salesTrend
        };
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnSale()
    {
        // Arrange
        var saleId = 1;
        var expectedSale = CreateRandomSaleDto(saleId);
        _saleServiceMock.Setup(x => x.GetByIdAsync(saleId))
            .ReturnsAsync(expectedSale);

        // Act
        var result = await _controller.GetById(saleId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var sale = okResult.Value.Should().BeOfType<SaleDto>().Subject;
        sale.Should().BeEquivalentTo(expectedSale);
        _output.WriteLine($"Получена продажа с ID {saleId}: {sale.OrderNumber}");
        _output.WriteLine($"Количество товаров: {sale.Products.Count}, Общая сумма: {sale.TotalAmount:N2}");
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllSales()
    {
        // Arrange
        var expectedSales = GenerateLargeSalesDataset();
        _saleServiceMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(expectedSales);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var sales = okResult.Value.Should().BeAssignableTo<IEnumerable<SaleDto>>().Subject;
        sales.Should().HaveCount(expectedSales.Count);
        
        var totalRevenue = sales.Sum(s => s.TotalAmount);
        var totalProducts = sales.Sum(s => s.Products.Count);
        var averageProductsPerSale = (double)totalProducts / sales.Count();
        
        _output.WriteLine($"Получено {sales.Count()} продаж");
        _output.WriteLine($"Общая выручка: {totalRevenue:N2}");
        _output.WriteLine($"Всего товаров: {totalProducts}");
        _output.WriteLine($"Среднее количество товаров в продаже: {averageProductsPerSale:F2}");
        
        // Дополнительная статистика
        var minProducts = sales.Min(s => s.Products.Count);
        var maxProducts = sales.Max(s => s.Products.Count);
        var minAmount = sales.Min(s => s.TotalAmount);
        var maxAmount = sales.Max(s => s.TotalAmount);
        var avgAmount = sales.Average(s => s.TotalAmount);
        
        _output.WriteLine("\nДополнительная статистика:");
        _output.WriteLine($"- Минимальное количество товаров в продаже: {minProducts}");
        _output.WriteLine($"- Максимальное количество товаров в продаже: {maxProducts}");
        _output.WriteLine($"- Минимальная сумма продажи: {minAmount:N2}");
        _output.WriteLine($"- Максимальная сумма продажи: {maxAmount:N2}");
        _output.WriteLine($"- Средняя сумма продажи: {avgAmount:N2}");
    }

    [Fact]
    public async Task GetSalesAnalytics_WithValidDates_ShouldReturnAnalytics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-90);
        var endDate = DateTime.UtcNow;
        var sales = GenerateLargeSalesDataset();
        var expectedAnalytics = CreateRealisticSalesAnalytics(sales, startDate, endDate);
        
        _saleServiceMock.Setup(x => x.GetSalesAnalyticsAsync(startDate, endDate))
            .ReturnsAsync(expectedAnalytics);

        // Act
        var result = await _controller.GetSalesAnalytics(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var analytics = okResult.Value.Should().BeOfType<SalesAnalyticsDto>().Subject;
        analytics.Should().BeEquivalentTo(expectedAnalytics);
        
        _output.WriteLine($"Получена аналитика за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
        _output.WriteLine($"Выручка: {analytics.Revenue:N2}");
        _output.WriteLine($"Количество продаж: {analytics.SalesCount}");
        _output.WriteLine($"Средний чек: {analytics.AverageSaleAmount:N2}");
        _output.WriteLine($"Количество категорий: {analytics.CategorySales.Count}");
        _output.WriteLine($"Топ категория: {analytics.CategorySales.First().Category} ({analytics.CategorySales.First().Revenue:N2})");
    }

    [Fact]
    public async Task GetDashboardAnalytics_WithValidDates_ShouldReturnAnalytics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var sales = GenerateLargeSalesDataset();
        var expectedAnalytics = CreateRealisticDashboardAnalytics(sales);
        
        _saleServiceMock.Setup(x => x.GetDashboardAnalyticsAsync(startDate, endDate))
            .ReturnsAsync(expectedAnalytics);

        // Act
        var result = await _controller.GetDashboardAnalytics(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var analytics = okResult.Value.Should().BeOfType<DashboardAnalyticsDto>().Subject;
        analytics.Should().BeEquivalentTo(expectedAnalytics);
        
        _output.WriteLine($"Получена аналитика дашборда за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
        _output.WriteLine($"Общая выручка: {analytics.TotalRevenue:N2}");
        _output.WriteLine($"Количество продаж: {analytics.TotalSalesCount}");
        _output.WriteLine($"Средний чек: {analytics.AverageOrderAmount:N2}");
        
        _output.WriteLine("\nТоп продуктов:");
        foreach (var product in analytics.TopProducts)
        {
            _output.WriteLine($"- {product.ProductName}: {product.SalesCount} продаж, {product.Revenue:N2} выручки");
        }
        
        _output.WriteLine("\nСтатистика по статусам заказов:");
        _output.WriteLine($"- Новых: {analytics.OrderStatusStats.NewCount}");
        _output.WriteLine($"- В обработке: {analytics.OrderStatusStats.ProcessingCount}");
        _output.WriteLine($"- Завершенных: {analytics.OrderStatusStats.CompletedCount}");
        _output.WriteLine($"- Отмененных: {analytics.OrderStatusStats.CancelledCount}");
    }

    [Fact]
    public async Task GetTopProducts_WithValidParameters_ShouldReturnTopProducts()
    {
        // Arrange
        var count = 20;
        var startDate = DateTime.UtcNow.AddDays(-90);
        var endDate = DateTime.UtcNow;
        var sales = GenerateLargeSalesDataset();
        
        var expectedProducts = sales
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .SelectMany(s => s.Products)
            .GroupBy(p => p.ProductName)
            .Select(g => new TopProductResultDto
            {
                ProductName = g.Key,
                SalesCount = g.Sum(p => p.Quantity),
                Revenue = g.Sum(p => p.ProductPrice * p.Quantity - p.DiscountAmount)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(count)
            .ToList();
            
        _saleServiceMock.Setup(x => x.GetTopProductsAsync(count, startDate, endDate))
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _controller.GetTopProducts(count, startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var products = okResult.Value.Should().BeAssignableTo<IEnumerable<TopProductResultDto>>().Subject;
        products.Should().HaveCount(count);
        
        _output.WriteLine($"Получено {products.Count()} топ продуктов за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
        _output.WriteLine("\nТоп продуктов:");
        foreach (var product in products)
        {
            _output.WriteLine($"- {product.ProductName}: {product.SalesCount} продаж, {product.Revenue:N2} выручки");
        }
        
        _output.WriteLine("\nСтатистика:");
        _output.WriteLine($"Лучший продукт: {products.First().ProductName}");
        _output.WriteLine($"- Продаж: {products.First().SalesCount}");
        _output.WriteLine($"- Выручка: {products.First().Revenue:N2}");
        _output.WriteLine($"- Доля в выручке: {(products.First().Revenue / products.Sum(p => p.Revenue)):P2}");
        
        _output.WriteLine($"\nХудший продукт в топе: {products.Last().ProductName}");
        _output.WriteLine($"- Продаж: {products.Last().SalesCount}");
        _output.WriteLine($"- Выручка: {products.Last().Revenue:N2}");
        _output.WriteLine($"- Доля в выручке: {(products.Last().Revenue / products.Sum(p => p.Revenue)):P2}");
        
        var totalRevenue = products.Sum(p => p.Revenue);
        var totalSales = products.Sum(p => p.SalesCount);
        _output.WriteLine($"\nОбщая статистика:");
        _output.WriteLine($"- Общая выручка: {totalRevenue:N2}");
        _output.WriteLine($"- Общее количество продаж: {totalSales}");
        _output.WriteLine($"- Средняя выручка на продукт: {(totalRevenue / products.Count()):N2}");
        _output.WriteLine($"- Среднее количество продаж на продукт: {(double)totalSales / products.Count():F2}");
    }

    [Fact]
    public async Task GetCategorySales_WithValidDates_ShouldReturnCategorySales()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-90);
        var endDate = DateTime.UtcNow;
        var sales = GenerateLargeSalesDataset();
        
        var expectedSales = sales
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .SelectMany(s => s.Products)
            .GroupBy(p => p.ProductCategory)
            .Select(g => new CategorySalesResultDto
            {
                Category = g.Key,
                SalesCount = g.Sum(p => p.Quantity),
                Revenue = g.Sum(p => p.ProductPrice * p.Quantity - p.DiscountAmount),
                Share = g.Sum(p => p.ProductPrice * p.Quantity - p.DiscountAmount) / 
                       sales.Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                           .Sum(s => s.TotalAmount)
            })
            .OrderByDescending(s => s.Revenue)
            .ToList();
            
        _saleServiceMock.Setup(x => x.GetCategorySalesAsync(startDate, endDate))
            .ReturnsAsync(expectedSales);

        // Act
        var result = await _controller.GetCategorySales(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categorySales = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorySalesResultDto>>().Subject;
        categorySales.Should().BeEquivalentTo(expectedSales);
        
        _output.WriteLine($"Получены продажи по {categorySales.Count()} категориям");
        _output.WriteLine($"Лучшая категория: {categorySales.First().Category}");
        _output.WriteLine($"Продаж: {categorySales.First().SalesCount}, Выручка: {categorySales.First().Revenue:N2}, Доля: {categorySales.First().Share:P2}");
        _output.WriteLine($"Худшая категория: {categorySales.Last().Category}");
        _output.WriteLine($"Продаж: {categorySales.Last().SalesCount}, Выручка: {categorySales.Last().Revenue:N2}, Доля: {categorySales.Last().Share:P2}");
    }

    [Fact]
    public async Task GetSalesTrend_WithValidParameters_ShouldReturnTrend()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var interval = "1d";
        var sales = GenerateLargeSalesDataset();
        
        var expectedTrend = new List<SalesTrendResultDto>();
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            var dailySales = sales.Where(s => s.SaleDate.Date == currentDate.Date).ToList();
            expectedTrend.Add(new SalesTrendResultDto
            {
                Date = currentDate,
                Revenue = dailySales.Sum(s => s.TotalAmount),
                SalesCount = dailySales.Count
            });
            currentDate = currentDate.AddDays(1);
        }
            
        _saleServiceMock.Setup(x => x.GetSalesTrendAsync(startDate, endDate, "1d"))
            .ReturnsAsync(expectedTrend);

        // Act
        var result = await _controller.GetSalesTrend(startDate, endDate, interval);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var trend = okResult.Value.Should().BeAssignableTo<IEnumerable<SalesTrendResultDto>>().Subject;
        trend.Should().HaveCount(expectedTrend.Count);
        
        _output.WriteLine($"Получен тренд за {trend.Count()} дней");
        _output.WriteLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
        _output.WriteLine($"Интервал: {interval}");
        
        _output.WriteLine("\nСтатистика по выручке:");
        _output.WriteLine($"- Максимальная выручка: {trend.Max(t => t.Revenue):N2}");
        _output.WriteLine($"- Минимальная выручка: {trend.Min(t => t.Revenue):N2}");
        _output.WriteLine($"- Средняя выручка: {trend.Average(t => t.Revenue):N2}");
        _output.WriteLine($"- Общая выручка: {trend.Sum(t => t.Revenue):N2}");
        
        _output.WriteLine("\nСтатистика по продажам:");
        _output.WriteLine($"- Максимальное количество продаж: {trend.Max(t => t.SalesCount)}");
        _output.WriteLine($"- Минимальное количество продаж: {trend.Min(t => t.SalesCount)}");
        _output.WriteLine($"- Среднее количество продаж: {trend.Average(t => t.SalesCount):F2}");
        _output.WriteLine($"- Общее количество продаж: {trend.Sum(t => t.SalesCount)}");
        
        _output.WriteLine("\nТоп дней по выручке:");
        foreach (var day in trend.OrderByDescending(t => t.Revenue).Take(5))
        {
            _output.WriteLine($"- {day.Date:dd.MM.yyyy}: {day.Revenue:N2} ({day.SalesCount} продаж)");
        }
        
        _output.WriteLine("\nТоп дней по количеству продаж:");
        foreach (var day in trend.OrderByDescending(t => t.SalesCount).Take(5))
        {
            _output.WriteLine($"- {day.Date:dd.MM.yyyy}: {day.SalesCount} продаж ({day.Revenue:N2})");
        }
    }

    [Fact]
    public async Task GetSalesTrend_WithFutureDate_ShouldReturnBadRequest()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var interval = "1d";

        // Act
        var result = await _controller.GetSalesTrend(startDate, endDate, interval);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest.Value.Should().Be("Начальная дата не может быть в будущем");
    }

    [Fact]
    public async Task GetTopProducts_WithInvalidCount_ShouldReturnBadRequest()
    {
        // Arrange
        var count = 0;
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        // Act
        var result = await _controller.GetTopProducts(count, startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest.Value.Should().Be("Количество продуктов должно быть больше 0");
    }

    [Fact]
    public async Task GetTopProducts_WithTooLargeCount_ShouldReturnBadRequest()
    {
        // Arrange
        var count = 101;
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        // Act
        var result = await _controller.GetTopProducts(count, startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest.Value.Should().Be("Количество продуктов не может быть больше 100");
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var id = 999;
        _saleServiceMock.Setup(x => x.GetByIdAsync(id))
            .ThrowsAsync(new SaleNotFoundException(id));

        // Act
        var result = await _controller.GetById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result.Result as NotFoundObjectResult;
        notFound.Value.Should().Be($"Продажа с ID {id} не найдена");
    }

    [Fact]
    public async Task CreateFromOrder_WithInvalidOrderId_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = 999;
        _saleServiceMock.Setup(x => x.CreateFromOrderAsync(orderId))
            .ThrowsAsync(new OrderNotFoundException(orderId));

        // Act
        var result = await _controller.CreateFromOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result.Result as NotFoundObjectResult;
        notFound.Value.Should().Be($"Заказ с ID {orderId} не найден");
    }

    [Fact]
    public async Task CreateFromOrder_WithInvalidOrderState_ShouldReturnBadRequest()
    {
        // Arrange
        var orderId = 1;
        var errorMessage = "Заказ не может быть преобразован в продажу";
        _saleServiceMock.Setup(x => x.CreateFromOrderAsync(orderId))
            .ThrowsAsync(new InvalidOrderStateException(errorMessage));

        // Act
        var result = await _controller.CreateFromOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetSalesAnalytics_WithInvalidDateRange_ShouldReturnBadRequest()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-1);
        var errorMessage = "Начальная дата не может быть позже конечной";

        // Act
        var result = await _controller.GetSalesAnalytics(startDate, endDate);

        // Assert
        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetDashboardAnalytics_WithEmptyData_ShouldReturnEmptyAnalytics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var emptyAnalytics = new DashboardAnalyticsDto
        {
            TotalRevenue = 0,
            TotalSalesCount = 0,
            AverageOrderAmount = 0,
            TotalOrdersCount = 0,
            TopProducts = new List<TopProductResultDto>(),
            OrderStatusStats = new OrderStatusStatsDto()
        };
        
        _saleServiceMock.Setup(x => x.GetDashboardAnalyticsAsync(startDate, endDate))
            .ReturnsAsync(emptyAnalytics);

        // Act
        var result = await _controller.GetDashboardAnalytics(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var analytics = okResult.Value.Should().BeOfType<DashboardAnalyticsDto>().Subject;
        analytics.Should().BeEquivalentTo(emptyAnalytics);
    }

    [Fact]
    public async Task GetSalesTrend_WithSingleSale_ShouldReturnCorrectTrend()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow;
        var interval = "1d";
        
        var sale = CreateRandomSaleDto(1);
        sale.SaleDate = startDate;
        var sales = new List<SaleDto> { sale };
        
        var expectedTrend = new List<SalesTrendResultDto>
        {
            new SalesTrendResultDto
            {
                Date = startDate,
                Revenue = sale.TotalAmount,
                SalesCount = 1
            }
        };
        
        _saleServiceMock.Setup(x => x.GetSalesTrendAsync(startDate, endDate, "1d"))
            .ReturnsAsync(expectedTrend);

        // Act
        var result = await _controller.GetSalesTrend(startDate, endDate, interval);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var trend = okResult.Value.Should().BeAssignableTo<IEnumerable<SalesTrendResultDto>>().Subject;
        trend.Should().BeEquivalentTo(expectedTrend);
    }

    [Fact]
    public async Task GetTopProducts_WithEqualSales_ShouldReturnCorrectOrder()
    {
        // Arrange
        var count = 3;
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var product1 = new TopProductResultDto { ProductName = "Product1", SalesCount = 10, Revenue = 1000 };
        var product2 = new TopProductResultDto { ProductName = "Product2", SalesCount = 10, Revenue = 1000 };
        var product3 = new TopProductResultDto { ProductName = "Product3", SalesCount = 10, Revenue = 1000 };
        
        var expectedProducts = new List<TopProductResultDto> { product1, product2, product3 };
        
        _saleServiceMock.Setup(x => x.GetTopProductsAsync(count, startDate, endDate))
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _controller.GetTopProducts(count, startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var products = okResult.Value.Should().BeAssignableTo<IEnumerable<TopProductResultDto>>().Subject;
        products.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetCategorySales_WithDifferentCategories_ShouldReturnCorrectShares()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var category1 = new CategorySalesResultDto 
        { 
            Category = "Category1", 
            SalesCount = 5, 
            Revenue = 500,
            Share = 0.5m
        };
        
        var category2 = new CategorySalesResultDto 
        { 
            Category = "Category2", 
            SalesCount = 5, 
            Revenue = 500,
            Share = 0.5m
        };
        
        var expectedCategories = new List<CategorySalesResultDto> { category1, category2 };
        
        _saleServiceMock.Setup(x => x.GetCategorySalesAsync(startDate, endDate))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _controller.GetCategorySales(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorySalesResultDto>>().Subject;
        categories.Should().BeEquivalentTo(expectedCategories);
        
        // Проверяем, что доли в сумме дают 1
        categories.Sum(c => c.Share).Should().Be(1m);
    }
} 