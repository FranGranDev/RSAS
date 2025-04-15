using Application.DTOs;
using Frontend.Models.Stocks;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Stocks;

public class ViewModel : PageModel
{
    private readonly IApiService _apiService;
    private readonly ILogger<ViewModel> _logger;

    public ViewModel(IApiService apiService, ILogger<ViewModel> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public StockViewModel StockViewModel { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            await LoadStockData();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке страницы склада {Id}", Id);
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAddProductAsync(int productId, int quantity)
    {
        try
        {
            if (quantity <= 0)
            {
                return BadRequest("Количество товара должно быть больше 0");
            }

            await _apiService.PostAsync<object, object>(
                $"api/stocks/{Id}/products/{productId}?quantity={quantity}",
                null);
            
            await LoadStockData();
            var product = StockViewModel.Products.FirstOrDefault(p => p.Product.Id == productId);
            if (product == null)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
            return Partial("Shared/Stocks/_StockProductPartial", product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении товара {ProductId} на склад {StockId}", productId, Id);
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> OnPostRemoveProductAsync(int productId)
    {
        try
        {
            await _apiService.DeleteAsync($"api/stocks/{Id}/products/{productId}");
            
            await LoadStockData();
            var product = StockViewModel.Products.FirstOrDefault(p => p.Product.Id == productId);
            if (product == null)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
            return Partial("Shared/Stocks/_StockProductPartial", product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении товара {ProductId} со склада {StockId}", productId, Id);
            return BadRequest("Ошибка при удалении товара");
        }
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int productId, int quantity)
    {
        try
        {
            if (quantity < 0)
            {
                return BadRequest("Количество товара не может быть отрицательным");
            }

            await _apiService.PutAsync<object, object>(
                $"api/stocks/{Id}/products/{productId}/quantity?quantity={quantity}",
                null);
            
            await LoadStockData();
            return Partial("Shared/Stocks/_StockProductPartial", StockViewModel.Products.First(p => p.Product.Id == productId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении количества товара {ProductId} на складе {StockId}", productId, Id);
            return BadRequest("Ошибка при обновлении количества");
        }
    }

    private async Task LoadStockData()
    {
        // Получаем информацию о складе
        var stock = await _apiService.GetAsync<StockDto>($"api/stocks/{Id}");
        if (stock == null)
        {
            throw new Exception($"Склад с ID {Id} не найден");
        }

        // Получаем товары на складе
        var stockProducts = await _apiService.GetAsync<IEnumerable<StockProductDto>>($"api/stocks/{Id}/products");
        
        // Получаем все товары
        var products = await _apiService.GetAsync<IEnumerable<ProductDto>>("api/products");

        // Создаем карточки товаров
        var productCards = products.Select(product =>
        {
            var stockProduct = stockProducts.FirstOrDefault(sp => sp.ProductId == product.Id);
            return new StockProductCardViewModel
            {
                Product = product,
                StockProduct = stockProduct,
                StockId = Id
            };
        });

        StockViewModel = new StockViewModel
        {
            Stock = stock,
            Products = productCards
        };
    }
} 