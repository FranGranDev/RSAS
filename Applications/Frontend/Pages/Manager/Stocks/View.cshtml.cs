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

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "name";

    [BindProperty(SupportsGet = true)]
    public string SortDirection { get; set; } = "asc";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 12;

    public StockViewModel StockViewModel { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Получаем информацию о складе
            var stock = await _apiService.GetAsync<StockDto>($"api/stocks/{Id}");
            if (stock == null)
            {
                return NotFound();
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
                    AddAction = $"api/stocks/{Id}/products/{product.Id}",
                    RemoveAction = $"api/stocks/{Id}/products/{product.Id}",
                    UpdateAction = $"api/stocks/{Id}/products/{product.Id}/quantity",
                    ManageUrl = Url.Page("/Manager/Stocks/View", new { id = Id })
                };
            });

            // Применяем фильтрацию и сортировку
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                productCards = productCards.Where(pc =>
                    pc.Product.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    pc.Product.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    pc.Product.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            productCards = SortBy switch
            {
                "name" => SortDirection == "asc" 
                    ? productCards.OrderBy(pc => pc.Product.Name)
                    : productCards.OrderByDescending(pc => pc.Product.Name),
                "category" => SortDirection == "asc"
                    ? productCards.OrderBy(pc => pc.Product.Category)
                    : productCards.OrderByDescending(pc => pc.Product.Category),
                "quantity" => SortDirection == "asc"
                    ? productCards.OrderBy(pc => pc.StockProduct?.Quantity ?? 0)
                    : productCards.OrderByDescending(pc => pc.StockProduct?.Quantity ?? 0),
                _ => productCards
            };

            // Применяем пагинацию
            var totalItems = productCards.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            productCards = productCards.Skip((PageNumber - 1) * PageSize).Take(PageSize);

            StockViewModel = new StockViewModel
            {
                Stock = stock,
                Products = productCards,
                SearchQuery = SearchQuery,
                SortBy = SortBy,
                SortDirection = SortDirection,
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalPages = totalPages,
                TotalItems = totalItems
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке страницы склада {Id}", Id);
            return RedirectToPage("/Error");
        }
    }
} 