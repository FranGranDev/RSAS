using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using AutoMapper;
using FluentAssertions;
using Moq;
using Server.Models;
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

    #region Базовые операции с продажами

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSale()
    {
        // Arrange
        var saleId = 1;
        var sale = new Sale { Id = saleId };
        var saleDto = new SaleDto { Id = saleId };

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId))
            .ReturnsAsync(sale);
        _mapperMock.Setup(x => x.Map<SaleDto>(sale))
            .Returns(saleDto);

        // Act
        var result = await _saleService.GetByIdAsync(saleId);

        // Assert
        result.Should().BeEquivalentTo(saleDto);
        _saleRepositoryMock.Verify(x => x.GetByIdAsync(saleId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSales()
    {
        // Arrange
        var sales = new List<Sale> { new Sale { Id = 1 }, new Sale { Id = 2 } };
        var saleDtos = new List<SaleDto> { new SaleDto { Id = 1 }, new SaleDto { Id = 2 } };

        _saleRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(sales);
        _mapperMock.Setup(x => x.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _saleService.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(saleDtos);
        _saleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateFromOrderAsync_WithExistingSale_ShouldThrowException()
    {
        // Arrange
        var orderId = 1;
        _saleRepositoryMock.Setup(x => x.ExistsByOrderIdAsync(orderId))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<OrderNotFoundException>(() =>
            _saleService.CreateFromOrderAsync(orderId));
    }

    [Fact]
    public async Task ExistsByOrderIdAsync_WithValidOrderId_ShouldReturnCorrectResult()
    {
        // Arrange
        var orderId = 1;
        _saleRepositoryMock.Setup(x => x.ExistsByOrderIdAsync(orderId))
            .ReturnsAsync(true);

        // Act
        var result = await _saleService.ExistsByOrderIdAsync(orderId);

        // Assert
        result.Should().BeTrue();
        _saleRepositoryMock.Verify(x => x.ExistsByOrderIdAsync(orderId), Times.Once);
    }

    #endregion

    #region Фильтрация продаж

    [Fact]
    public async Task GetByDateRangeAsync_WithValidDates_ShouldReturnSales()
    {
        // Arrange
        var sales = new List<Sale> { new Sale { Id = 1 }, new Sale { Id = 2 } };
        var saleDtos = new List<SaleDto> { new SaleDto { Id = 1 }, new SaleDto { Id = 2 } };

        _saleRepositoryMock.Setup(x => x.GetByDateRangeAsync(_startDate, _endDate))
            .ReturnsAsync(sales);
        _mapperMock.Setup(x => x.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _saleService.GetByDateRangeAsync(_startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(saleDtos);
        _saleRepositoryMock.Verify(x => x.GetByDateRangeAsync(_startDate, _endDate), Times.Once);
    }

    [Fact]
    public async Task GetByClientAsync_WithValidPhone_ShouldReturnSales()
    {
        // Arrange
        var clientPhone = "+1234567890";
        var sales = new List<Sale> { new Sale { Id = 1 }, new Sale { Id = 2 } };
        var saleDtos = new List<SaleDto> { new SaleDto { Id = 1 }, new SaleDto { Id = 2 } };

        _saleRepositoryMock.Setup(x => x.GetByClientAsync(clientPhone))
            .ReturnsAsync(sales);
        _mapperMock.Setup(x => x.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _saleService.GetByClientAsync(clientPhone);

        // Assert
        result.Should().BeEquivalentTo(saleDtos);
        _saleRepositoryMock.Verify(x => x.GetByClientAsync(clientPhone), Times.Once);
    }

    [Fact]
    public async Task GetByProductAsync_WithValidProductId_ShouldReturnSales()
    {
        // Arrange
        var productId = 1;
        var sales = new List<Sale> { new Sale { Id = 1 }, new Sale { Id = 2 } };
        var saleDtos = new List<SaleDto> { new SaleDto { Id = 1 }, new SaleDto { Id = 2 } };

        _saleRepositoryMock.Setup(x => x.GetByProductAsync(productId))
            .ReturnsAsync(sales);
        _mapperMock.Setup(x => x.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _saleService.GetByProductAsync(productId);

        // Assert
        result.Should().BeEquivalentTo(saleDtos);
        _saleRepositoryMock.Verify(x => x.GetByProductAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetByCategoryAsync_WithValidCategory_ShouldReturnSales()
    {
        // Arrange
        var category = "Category1";
        var sales = new List<Sale> { new Sale { Id = 1 }, new Sale { Id = 2 } };
        var saleDtos = new List<SaleDto> { new SaleDto { Id = 1 }, new SaleDto { Id = 2 } };

        _saleRepositoryMock.Setup(x => x.GetByCategoryAsync(category))
            .ReturnsAsync(sales);
        _mapperMock.Setup(x => x.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _saleService.GetByCategoryAsync(category);

        // Assert
        result.Should().BeEquivalentTo(saleDtos);
        _saleRepositoryMock.Verify(x => x.GetByCategoryAsync(category), Times.Once);
    }

    #endregion

    #region Базовая аналитика

    [Fact]
    public async Task GetTotalRevenueAsync_WithValidDates_ShouldReturnCorrectAmount()
    {
        // Arrange
        var expectedRevenue = 10000m;
        _saleRepositoryMock.Setup(x => x.GetTotalRevenueAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))))
            .ReturnsAsync(expectedRevenue);

        // Act
        var result = await _saleService.GetTotalRevenueAsync(_startDate, _endDate);

        // Assert
        result.Should().Be(expectedRevenue);
        _saleRepositoryMock.Verify(x => x.GetTotalRevenueAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))), Times.Once);
    }

    [Fact]
    public async Task GetTotalSalesCountAsync_WithValidDates_ShouldReturnCorrectCount()
    {
        // Arrange
        var expectedCount = 100;
        _saleRepositoryMock.Setup(x => x.GetTotalSalesCountAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _saleService.GetTotalSalesCountAsync(_startDate, _endDate);

        // Assert
        result.Should().Be(expectedCount);
        _saleRepositoryMock.Verify(x => x.GetTotalSalesCountAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))), Times.Once);
    }

    #endregion

    #region Аналитика по продуктам

    [Fact]
    public async Task GetCategorySalesAsync_WithValidDates_ShouldReturnCorrectCategories()
    {
        // Arrange
        var expectedCategories = new List<CategorySalesResultDto>
        {
            new() { Category = "Category1", SalesCount = 50, Revenue = 5000m, Share = 0.5m },
            new() { Category = "Category2", SalesCount = 30, Revenue = 3000m, Share = 0.3m }
        };

        _saleRepositoryMock.Setup(x => x.GetCategorySalesAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _saleService.GetCategorySalesAsync(_startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedCategories);
        _saleRepositoryMock.Verify(x => x.GetCategorySalesAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))), Times.Once);
    }

    #endregion

    #region Аналитика по времени

    [Fact]
    public async Task GetSalesTrendAsync_WithValidParameters_ShouldReturnCorrectTrend()
    {
        // Arrange
        var expectedTrend = new List<SalesTrendResultDto>
        {
            new() { Date = _startDate, Revenue = 1000m, SalesCount = 10 },
            new() { Date = _startDate.AddDays(1), Revenue = 2000m, SalesCount = 20 }
        };

        _saleRepositoryMock.Setup(x => x.GetSalesTrendAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1)),
            It.Is<TimeSpan>(t => t == TimeSpan.FromDays(1))))
            .ReturnsAsync(expectedTrend);

        // Act
        var result = await _saleService.GetSalesTrendAsync(_startDate, _endDate, "1d");

        // Assert
        result.Should().BeEquivalentTo(expectedTrend);
        _saleRepositoryMock.Verify(x => x.GetSalesTrendAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1)),
            It.Is<TimeSpan>(t => t == TimeSpan.FromDays(1))), Times.Once);
    }

    [Fact]
    public async Task GetSalesTrendAsync_WithInvalidInterval_ShouldThrowException()
    {
        // Arrange
        var invalidInterval = "invalid";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidAnalyticsParametersException>(() =>
            _saleService.GetSalesTrendAsync(_startDate, _endDate, invalidInterval));
    }

    #endregion
    

    #region Расширенная аналитика

    [Fact]
    public async Task GetExtendedAnalyticsAsync_WithValidDates_ShouldReturnCorrectAnalytics()
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

        _saleRepositoryMock.Setup(x => x.GetSalesConversionRateAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(expectedAnalytics.ConversionRate);
        _saleRepositoryMock.Setup(x => x.GetAverageOrderProcessingTimeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(expectedAnalytics.AverageOrderProcessingTime);
        _saleRepositoryMock.Setup(x => x.GetStockEfficiencyAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(expectedAnalytics.StockEfficiency);
        _saleRepositoryMock.Setup(x => x.GetSeasonalityAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedAnalytics.Seasonality);
        _saleRepositoryMock.Setup(x => x.GetSalesForecastAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedAnalytics.SalesForecast);

        // Act
        var result = await _saleService.GetExtendedAnalyticsAsync(_startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedAnalytics);
    }

    [Fact]
    public async Task GetKpiAsync_WithValidDates_ShouldReturnCorrectKpi()
    {
        // Arrange
        var expectedKpi = new KpiDto
        {
            SalesConversion = 0.8m,
            AverageOrderTime = TimeSpan.FromHours(2),
            Revenue = 10000m,
            SalesVolume = 100,
            AverageCheck = 100m,
            StockTurnover = 0.5m,
            AverageOrderProcessingTime = TimeSpan.FromHours(2)
        };

        _saleRepositoryMock.Setup(x => x.GetKpiAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))))
            .ReturnsAsync(expectedKpi);

        // Act
        var result = await _saleService.GetKpiAsync(_startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedKpi);
        _saleRepositoryMock.Verify(x => x.GetKpiAsync(
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))), Times.Once);
    }

    #endregion

    #region Прогнозирование

    [Fact]
    public async Task GetDemandForecastAsync_WithValidParameters_ShouldReturnCorrectForecast()
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
                RecommendedOrder = 60,
                ForecastedRevenue = 1000m,
                Confidence = 0.8m,
                Message = "Запасы в норме"
            }
        };

        _saleRepositoryMock.Setup(x => x.GetDemandForecastAsync(
            It.Is<int>(d => d == days),
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _saleService.GetDemandForecastAsync(days, _startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedForecast);
        _saleRepositoryMock.Verify(x => x.GetDemandForecastAsync(
            It.Is<int>(d => d == days),
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))), Times.Once);
    }

    [Fact]
    public async Task GetDemandForecastAsync_WithInvalidDays_ShouldThrowException()
    {
        // Arrange
        var days = 0;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidAnalyticsParametersException>(() =>
            _saleService.GetDemandForecastAsync(days, _startDate, _endDate));
    }

    [Fact]
    public async Task GetSeasonalityImpactAsync_WithValidParameters_ShouldReturnCorrectImpact()
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

        _saleRepositoryMock.Setup(x => x.GetSeasonalityImpactAsync(
            It.Is<int>(y => y == years),
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))))
            .ReturnsAsync(expectedImpact);

        // Act
        var result = await _saleService.GetSeasonalityImpactAsync(years, _startDate, _endDate);

        // Assert
        result.Should().BeEquivalentTo(expectedImpact);
        _saleRepositoryMock.Verify(x => x.GetSeasonalityImpactAsync(
            It.Is<int>(y => y == years),
            It.Is<DateTime>(d => d == _startDate),
            It.Is<DateTime>(d => d == _endDate.Date.AddDays(1).AddTicks(-1))), Times.Once);
    }

    [Fact]
    public async Task GetSeasonalityImpactAsync_WithInvalidYears_ShouldThrowException()
    {
        // Arrange
        var years = 0;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidAnalyticsParametersException>(() =>
            _saleService.GetSeasonalityImpactAsync(years, _startDate, _endDate));
    }

    #endregion
} 