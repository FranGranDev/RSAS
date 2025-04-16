using Frontend.Models.Catalog;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using Application.DTOs;

namespace Frontend.Pages.Client.Catalog;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMemoryCache _memoryCache;
    private readonly IApiService _apiService;
    private const string CartCacheKey = "Cart_{0}"; // {0} будет заменено на UserId

    public IndexModel(IMemoryCache memoryCache, IApiService apiService)
    {
        _memoryCache = memoryCache;
        _apiService = apiService;
    }

    public CatalogViewModel Catalog { get; set; }

    public async Task OnGetAsync()
    {
        // Получаем список товаров из API
        var products = await _apiService.GetAsync<List<ProductDto>>("api/products");
        
        // Преобразуем DTO в ViewModel
        var productViewModels = products.Select(p => new CatalogProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category,
            Description = p.Description,
            Barcode = p.Barcode,
            Price = p.Price,
            Quantity = 0 // Начальное количество в корзине
        }).ToList();

        // Получаем уникальные категории
        var categories = products.Select(p => p.Category)
                               .Distinct()
                               .OrderBy(c => c)
                               .ToList();

        Catalog = new CatalogViewModel
        {
            Products = productViewModels,
            Categories = categories,
            Cart = GetOrCreateCart()
        };
    }

    private CartViewModel GetOrCreateCart()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var cacheKey = string.Format(CartCacheKey, userId);
        
        if (!_memoryCache.TryGetValue(cacheKey, out CartViewModel cart))
        {
            cart = new CartViewModel();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));
            _memoryCache.Set(cacheKey, cart, cacheOptions);
        }

        return cart;
    }

    public async Task<IActionResult> OnPostUpdateCart([FromBody] UpdateCartRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var cacheKey = string.Format(CartCacheKey, userId);
        
        if (!_memoryCache.TryGetValue(cacheKey, out CartViewModel cart))
        {
            cart = new CartViewModel();
        }

        var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);
        if (item != null)
        {
            if (request.Quantity == 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
            }
        }
        else if (request.Quantity > 0)
        {
            // Получаем информацию о товаре из API
            var product = await _apiService.GetAsync<ProductDto>($"api/products/{request.ProductId}");
            
            cart.Items.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                Price = product.Price,
                Quantity = request.Quantity
            });
        }

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(7));
        _memoryCache.Set(cacheKey, cart, cacheOptions);

        return new JsonResult(new
        {
            totalQuantity = cart.TotalQuantity,
            totalPrice = cart.TotalPrice
        });
    }

    public IActionResult OnPostClearCart()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var cacheKey = string.Format(CartCacheKey, userId);
        
        var cart = new CartViewModel();
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(7));
        _memoryCache.Set(cacheKey, cart, cacheOptions);

        return new JsonResult(new
        {
            totalQuantity = cart.TotalQuantity,
            totalPrice = cart.TotalPrice
        });
    }
}

public class UpdateCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
} 