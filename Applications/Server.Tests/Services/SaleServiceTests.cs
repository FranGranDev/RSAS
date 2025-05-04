using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using AutoMapper;
using FluentAssertions;
using Moq;
using Server.Services.Repository;
using Server.Services.Sales;
using Xunit;

namespace Server.Tests.Services;

public class SaleServiceTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SaleService _saleService;
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public SaleServiceTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _mapperMock = new Mock<IMapper>();
        _saleService = new SaleService(_saleRepositoryMock.Object, _orderRepositoryMock.Object, _mapperMock.Object);
        
        _startDate = DateTime.UtcNow.AddDays(-30);
        _endDate = DateTime.UtcNow;
    }

    [Fact]
    public async Task GetTotalRevenue_WithValidDates_ShouldReturnCorrectAmount()
    {
        // Arrange
        var expectedRevenue = 10000m;
        _saleRepositoryMock.Setup(x => x.GetTotalRevenueAsync(_startDate, _endDate))
            .ReturnsAsync(expectedRevenue);

        // Act
        var result = await _saleService.GetTotalRevenueAsync(_startDate, _endDate);

        // Assert
        result.Should().Be(expectedRevenue);
        _saleRepositoryMock.Verify(x => x.GetTotalRevenueAsync(_startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetTotalSalesCount_WithValidDates_ShouldReturnCorrectCount()
    {
        // Arrange
        var expectedCount = 100;
        _saleRepositoryMock.Setup(x => x.GetTotalSalesCountAsync(_startDate, _endDate))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _saleService.GetTotalSalesCountAsync(_startDate, _endDate);

        // Assert
        result.Should().Be(expectedCount);
        _saleRepositoryMock.Verify(x => x.GetTotalSalesCountAsync(_startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetAverageSaleAmount_WithValidDates_ShouldReturnCorrectAmount()
    {
        // Arrange
        var expectedAmount = 100m;
        _saleRepositoryMock.Setup(x => x.GetAverageSaleAmountAsync(_startDate, _endDate))
            .ReturnsAsync(expectedAmount);

        // Act
        var result = await _saleService.GetAverageSaleAmountAsync(_startDate, _endDate);

        // Assert
        result.Should().Be(expectedAmount);
        _saleRepositoryMock.Verify(x => x.GetAverageSaleAmountAsync(_startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetTopProducts_WithValidParameters_ShouldReturnCorrectProducts()
    {
        // Arrange
        var count = 5;
        var expectedProducts = new List<TopProductResultDto>
        {
            new() { ProductName = "Product1", SalesCount = 100, Revenue = 1000m },
            new() { ProductName = "Product2", SalesCount = 90, Revenue = 900m }
        };

        _saleRepositoryMock.Setup(x => x.GetTopProductsAsync(count, _startDate, _endDate))
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _saleService.GetTopProductsAsync(count, _startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedProducts);
        _saleRepositoryMock.Verify(x => x.GetTopProductsAsync(count, _startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetCategorySales_WithValidDates_ShouldReturnCorrectCategories()
    {
        // Arrange
        var expectedCategories = new List<CategorySalesResultDto>
        {
            new() { Category = "Category1", SalesCount = 50, Revenue = 5000m, Share = 0.5m },
            new() { Category = "Category2", SalesCount = 30, Revenue = 3000m, Share = 0.3m }
        };

        _saleRepositoryMock.Setup(x => x.GetCategorySalesAsync(_startDate, _endDate))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _saleService.GetCategorySalesAsync(_startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedCategories);
        _saleRepositoryMock.Verify(x => x.GetCategorySalesAsync(_startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetSalesTrend_WithValidParameters_ShouldReturnCorrectTrend()
    {
        // Arrange
        var expectedTrend = new List<SalesTrendResultDto>
        {
            new() { Date = _startDate, Revenue = 1000m, SalesCount = 10 },
            new() { Date = _startDate.AddDays(1), Revenue = 2000m, SalesCount = 20 }
        };

        _saleRepositoryMock.Setup(x => x.GetSalesTrendAsync(_startDate, _endDate, TimeSpan.FromDays(1)))
            .ReturnsAsync(expectedTrend);

        // Act
        var result = await _saleService.GetSalesTrendAsync(_startDate, _endDate, "1d");

        // Assert
        result.Should().BeEquivalentTo(expectedTrend);
        _saleRepositoryMock.Verify(x => x.GetSalesTrendAsync(_startDate, _endDate, TimeSpan.FromDays(1)), Times.Once);
    }

    [Fact]
    public async Task GetExtendedAnalytics_WithValidDates_ShouldReturnCorrectAnalytics()
    {
        // Arrange
        var expectedAnalytics = new ExtendedSalesAnalyticsDto
        {
            ConversionRate = 0.8m,
            AverageOrderProcessingTime = TimeSpan.FromHours(2),
            StockEfficiency = new List<StockEfficiencyResultDto>
            {
                new() { StockName = "Stock1", Turnover = 5000m, TurnoverRatio = 0.5m }
            },
            Seasonality = new List<SeasonalityResultDto>
            {
                new() { Period = "Q1", AverageSales = 1000m, Deviation = 100m, SeasonalityIndex = 1.1m }
            },
            SalesForecast = new List<SalesForecastResultDto>
            {
                new() { Date = _endDate.AddDays(1), ForecastedSales = 1100m, LowerBound = 1000m, UpperBound = 1200m }
            }
        };

        _saleRepositoryMock.Setup(x => x.GetSalesConversionRateAsync(_startDate, _endDate))
            .ReturnsAsync(expectedAnalytics.ConversionRate);
        _saleRepositoryMock.Setup(x => x.GetAverageOrderProcessingTimeAsync(_startDate, _endDate))
            .ReturnsAsync(expectedAnalytics.AverageOrderProcessingTime);
        _saleRepositoryMock.Setup(x => x.GetStockEfficiencyAsync(_startDate, _endDate))
            .ReturnsAsync(expectedAnalytics.StockEfficiency);
        _saleRepositoryMock.Setup(x => x.GetSeasonalityAsync(3))
            .ReturnsAsync(expectedAnalytics.Seasonality);
        _saleRepositoryMock.Setup(x => x.GetSalesForecastAsync(30))
            .ReturnsAsync(expectedAnalytics.SalesForecast);

        // Act
        var result = await _saleService.GetExtendedAnalyticsAsync(_startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedAnalytics);
    }

    [Fact]
    public async Task GetDemandForecast_WithValidParameters_ShouldReturnCorrectForecast()
    {
        // Arrange
        var days = 30;
        var expectedForecast = new List<DemandForecastDto>
        {
            new() 
            { 
                ProductName = "Product1",
                Category = "Category1",
                ForecastedQuantity = 100,
                LowerBound = 90,
                UpperBound = 110,
                CurrentStock = 50,
                RecommendedOrder = 60
            }
        };

        _saleRepositoryMock.Setup(x => x.GetDemandForecastAsync(days, _startDate, _endDate))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _saleService.GetDemandForecastAsync(days, _startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedForecast);
        _saleRepositoryMock.Verify(x => x.GetDemandForecastAsync(days, _startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetSeasonalityImpact_WithValidParameters_ShouldReturnCorrectImpact()
    {
        // Arrange
        var years = 3;
        var expectedImpact = new List<SeasonalityImpactDto>
        {
            new() 
            { 
                Category = "Category1",
                SeasonalityIndex = 1.2m,
                PeakMonth = "December",
                LowMonth = "January",
                Impact = 0.3m
            }
        };

        _saleRepositoryMock.Setup(x => x.GetSeasonalityImpactAsync(years, _startDate, _endDate))
            .ReturnsAsync(expectedImpact);

        // Act
        var result = await _saleService.GetSeasonalityImpactAsync(years, _startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedImpact);
        _saleRepositoryMock.Verify(x => x.GetSeasonalityImpactAsync(years, _startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetDemandForecast_WithInvalidDays_ShouldThrowException()
    {
        // Arrange
        var days = 0;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidAnalyticsParametersException>(() =>
            _saleService.GetDemandForecastAsync(days, _startDate, _endDate));
    }

    [Fact]
    public async Task GetSeasonalityImpact_WithInvalidYears_ShouldThrowException()
    {
        // Arrange
        var years = 0;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidAnalyticsParametersException>(() =>
            _saleService.GetSeasonalityImpactAsync(years, _startDate, _endDate));
    }
} 