using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Application.Tests.Integration.Areas.Admin.Pages.Analytics;

public class AnalyticsPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AnalyticsPageTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task OnGet_ShouldReturnPage()
    {
        // Act
        var response = await _client.GetAsync("/Admin/Analytics/Index");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("По продажам", content); // Проверяем наличие элементов интерфейса
    }

    [Fact]
    public async Task OnGetSales_ShouldReturnSalesData()
    {
        // Arrange
        var requestUri = "/Admin/Analytics/Index?handler=GetSales";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("_SalesPartial", content); // Проверяем загрузку частичного представления
    }

    [Fact]
    public async Task OnPostSort_ShouldSortSalesByPriceAsc()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "SortBy", "price" },
            { "SortOrder", "asc" }
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Admin/Analytics/Index?handler=Sort", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        Assert.Contains("_SalesPartial", result);
        Assert.Contains("Отсортировано по цене", result); // Убедимся в сортировке
    }

    [Fact]
    public async Task OnPostSetDate_ShouldFilterSalesByDate()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "StartDate", "2024-01-01" },
            { "EndDate", "2024-12-31" }
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Admin/Analytics/Index?handler=SetDate", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        Assert.Contains("_SalesPartial", result);
        Assert.Contains("с 2024-01-01 по 2024-12-31", result); // Проверяем дату
    }

    [Fact]
    public async Task OnGetAnalytics_ShouldReturnAnalyticsPartial()
    {
        // Act
        var response = await _client.GetAsync("/Admin/Analytics/Index?handler=Analytics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("_SalesAnalyticsPartial", content); // Проверяем частичное представление
    }

    [Fact]
    public async Task OnGetInfo_ShouldReturnSaleInfo()
    {
        // Act
        var response = await _client.GetAsync("/Admin/Analytics/Index?handler=Info&id=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("_SaleInfoPartial", content); // Проверяем частичное представление
    }
}
