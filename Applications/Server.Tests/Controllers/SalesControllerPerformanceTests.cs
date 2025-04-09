using Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Server.Controllers;
using Server.Services.Sales;
using System.Diagnostics;

namespace Server.Tests.Controllers
{
    public class SalesControllerPerformanceTests
    {
        private readonly SalesController _controller;
        private readonly Mock<ISaleService> _saleServiceMock;
        private readonly Random _random = new();

        public SalesControllerPerformanceTests()
        {
            _saleServiceMock = new Mock<ISaleService>();
            _controller = new SalesController(_saleServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_WithLargeDataset_ShouldReturnInLessThan5Seconds()
        {
            // Arrange
            var largeDataset = GenerateLargeSalesDataset(1000000);
            _saleServiceMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(largeDataset);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var result = await _controller.GetAll();
            stopwatch.Stop();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var sales = okResult.Value.Should().BeAssignableTo<IEnumerable<SaleDto>>().Subject;
            sales.Should().HaveCount(1000000);
            
            // Проверяем время выполнения
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000);
            
            // Выводим статистику
            Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine($"Количество продаж: {sales.Count():N0}");
            Console.WriteLine($"Средний размер продажи: {sales.Average(s => s.Products.Count):F2} товаров");
            Console.WriteLine($"Общая выручка: {sales.Sum(s => s.TotalAmount):C}");
            Console.WriteLine($"Средний чек: {sales.Average(s => s.TotalAmount):C}");
        }

        [Fact]
        public async Task GetSalesAnalytics_WithLargeDateRange_ShouldReturnInLessThan10Seconds()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddYears(-1);
            var endDate = DateTime.UtcNow;
            
            var analytics = new SalesAnalyticsDto
            {
                Period = "1 год",
                Revenue = 100000000,
                SalesCount = 1000000,
                AverageSaleAmount = 100,
                CategorySales = GenerateLargeCategorySales(),
                SalesTrend = GenerateLargeSalesTrend(startDate, endDate)
            };

            _saleServiceMock.Setup(x => x.GetSalesAnalyticsAsync(startDate, endDate))
                .ReturnsAsync(analytics);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var result = await _controller.GetSalesAnalytics(startDate, endDate);
            stopwatch.Stop();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var resultAnalytics = okResult.Value.Should().BeOfType<SalesAnalyticsDto>().Subject;
            
            // Проверяем время выполнения
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000);
            
            // Выводим статистику
            Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine($"Количество категорий: {resultAnalytics.CategorySales.Count}");
            Console.WriteLine($"Количество точек тренда: {resultAnalytics.SalesTrend.Count}");
            Console.WriteLine($"Общая выручка: {resultAnalytics.Revenue:C}");
            Console.WriteLine($"Средний чек: {resultAnalytics.AverageSaleAmount:C}");
        }

        [Fact]
        public async Task GetTopProducts_WithLargeDataset_ShouldReturnInLessThan3Seconds()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddMonths(-6);
            var endDate = DateTime.UtcNow;
            var count = 100;
            
            var topProducts = GenerateLargeTopProducts(count);
            _saleServiceMock.Setup(x => x.GetTopProductsAsync(count, startDate, endDate))
                .ReturnsAsync(topProducts);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var result = await _controller.GetTopProducts(count, startDate, endDate);
            stopwatch.Stop();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var products = okResult.Value.Should().BeAssignableTo<IEnumerable<TopProductResultDto>>().Subject;
            products.Should().HaveCount(count);
            
            // Проверяем время выполнения
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000);
            
            // Выводим статистику
            Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine($"Количество продуктов: {products.Count():N0}");
            Console.WriteLine($"Общая выручка: {products.Sum(p => p.Revenue):C}");
            Console.WriteLine($"Средняя выручка на продукт: {products.Average(p => p.Revenue):C}");
        }

        private List<SaleDto> GenerateLargeSalesDataset(int count)
        {
            var sales = new List<SaleDto>();
            for (int i = 1; i <= count; i++)
            {
                sales.Add(CreateRandomSaleDto(i));
            }
            return sales;
        }

        private List<CategorySalesResultDto> GenerateLargeCategorySales()
        {
            var categories = new List<string> { "Электроника", "Одежда", "Продукты", "Книги", "Спорт" };
            return categories.Select(c => new CategorySalesResultDto
            {
                Category = c,
                SalesCount = _random.Next(1000, 5000),
                Revenue = _random.Next(100000, 500000),
                Share = _random.Next(10, 30) / 100m
            }).ToList();
        }

        private List<SalesTrendResultDto> GenerateLargeSalesTrend(DateTime startDate, DateTime endDate)
        {
            var trend = new List<SalesTrendResultDto>();
            var currentDate = startDate;
            
            while (currentDate <= endDate)
            {
                trend.Add(new SalesTrendResultDto
                {
                    Date = currentDate,
                    Revenue = _random.Next(10000, 50000),
                    SalesCount = _random.Next(100, 500)
                });
                currentDate = currentDate.AddDays(1);
            }
            
            return trend;
        }

        private List<TopProductResultDto> GenerateLargeTopProducts(int count)
        {
            return Enumerable.Range(1, count)
                .Select(i => new TopProductResultDto
                {
                    ProductName = $"Товар {i}",
                    SalesCount = _random.Next(100, 1000),
                    Revenue = _random.Next(10000, 100000)
                })
                .OrderByDescending(p => p.Revenue)
                .ToList();
        }

        private SaleDto CreateRandomSaleDto(int id)
        {
            var productCount = _random.Next(1, 10);
            var products = Enumerable.Range(1, productCount)
                .Select(i => new SaleProductDto
                {
                    Id = i,
                    SaleId = id,
                    ProductId = _random.Next(1, 100),
                    ProductName = $"Товар {_random.Next(1, 100)}",
                    ProductCategory = $"Категория {_random.Next(1, 5)}",
                    Quantity = _random.Next(1, 5),
                    ProductPrice = _random.Next(100, 1000),
                    DiscountAmount = _random.Next(0, 100)
                }).ToList();

            return new SaleDto
            {
                Id = id,
                OrderId = _random.Next(1, 1000),
                OrderNumber = $"ORD-{_random.Next(1000, 9999)}",
                StockId = _random.Next(1, 5),
                StockName = $"Склад {_random.Next(1, 5)}",
                SaleDate = DateTime.UtcNow.AddDays(-_random.Next(0, 365)),
                TotalAmount = products.Sum(p => p.ProductPrice * p.Quantity - p.DiscountAmount),
                ClientName = $"Клиент {_random.Next(1, 100)}",
                ClientPhone = $"+37529{_random.Next(1000000, 9999999)}",
                Products = products
            };
        }
    }
} 