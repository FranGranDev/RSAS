using Application.Model.Orders;
using Application.Model.Sales;

public class AnalyticsServiceTests
{
    private AnalyticsService _analyticsService;

    public AnalyticsServiceTests()
    {
        _analyticsService = new AnalyticsService();
    }

    public static IEnumerable<object[]> CalculateIncomeTestData =>
        new List<object[]>
        {
            new object[]
            {
                new Sale[] { }, 0m
            },
            new object[]
            {
                new Sale[]
                {
                    new Sale
                    {
                        Order = new Order
                        {
                            Products = new List<OrderProduct>
                            {
                                new OrderProduct { ProductPrice = 1m, Quantity = 2 },
                                new OrderProduct { ProductPrice = 2m, Quantity = 3 }
                            }
                        }
                    },
                    new Sale
                    {
                        Order = new Order
                        {
                            Products = new List<OrderProduct>
                            {
                                new OrderProduct { ProductPrice = 3m, Quantity = 1 }
                            }
                        }
                    }
                },
                (1m * 2) + (2m * 3) + (3m * 1)
            },
            new object[]
            {
                new Sale[]
                {
                    new Sale
                    {
                        Order = new Order
                        {
                            Products = new List<OrderProduct>
                            {
                                new OrderProduct { ProductPrice = 23m, Quantity = 1 },
                                new OrderProduct { ProductPrice = 54m, Quantity = 2 }
                            }
                        }
                    },
                    new Sale
                    {
                        Order = new Order
                        {
                            Products = new List<OrderProduct>
                            {
                                new OrderProduct { ProductPrice = 324m, Quantity = 1 },
                                new OrderProduct { ProductPrice = 54m, Quantity = 1 },
                                new OrderProduct { ProductPrice = 1m, Quantity = 3 },
                                new OrderProduct { ProductPrice = 54m, Quantity = 1 },
                                new OrderProduct { ProductPrice = 93m, Quantity = 2 }
                            }
                        }
                    }
                },
                (23m * 1) + (54m * 2) + (324m * 1) + (54m * 1) + (1m * 3) + (54m * 1) + (93m * 2)
            }
        };

    [Theory]
    [MemberData(nameof(CalculateIncomeTestData))]
    public void CalculateIncome_ShouldReturnCorrectTotalIncome(Sale[] sales, decimal expectedIncome)
    {
        // Act
        var actualIncome = _analyticsService.CalculateIncome(sales.ToList());

        // Assert
        Assert.Equal(expectedIncome, actualIncome);
    }

    [Fact]
    public void CalculateAverageBill_ShouldReturnCorrectAverage()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new Sale
            {
                Order = new Order
                {
                    Products = new List<OrderProduct>
                    {
                        new OrderProduct { ProductPrice = 100m },
                        new OrderProduct { ProductPrice = 200m }
                    }
                }
            },
            new Sale
            {
                Order = new Order
                {
                    Products = new List<OrderProduct>
                    {
                        new OrderProduct { ProductPrice = 300m }
                    }
                }
            }
        };

        var expectedAverageBill = (100m + 200m + 300m) / 2;

        // Act
        var actualAverageBill = _analyticsService.CalculateAverageBill(sales);

        // Assert
        Assert.Equal(expectedAverageBill, actualAverageBill);
    }

    [Fact]
    public void CalculateProductsCount_ShouldReturnCorrectCount()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new Sale
            {
                Order = new Order
                {
                    Products = new List<OrderProduct>
                    {
                        new OrderProduct(),
                        new OrderProduct()
                    }
                }
            },
            new Sale
            {
                Order = new Order
                {
                    Products = new List<OrderProduct>
                    {
                        new OrderProduct()
                    }
                }
            }
        };

        var expectedProductCount = 3;

        // Act
        var actualProductCount = _analyticsService.CalculateProductsCount(sales);

        // Assert
        Assert.Equal(expectedProductCount, actualProductCount);
    }

    [Fact]
    public void CalculateSalesCount_ShouldReturnCorrectCount()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new Sale(),
            new Sale(),
            new Sale()
        };

        var expectedSalesCount = 3;

        // Act
        var actualSalesCount = _analyticsService.CalculateSalesCount(sales);

        // Assert
        Assert.Equal(expectedSalesCount, actualSalesCount);
    }
}