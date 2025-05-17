using Application.DTOs;
using Frontend.Models.Catalog;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace Frontend.Pages.Manager.FastOrder
{
    [Authorize(Policy = "RequireManagerRole")]
    public class CatalogModel : PageModel
    {
        private readonly IApiService _apiService;
        private readonly IMemoryCache _memoryCache;
        private const string FastOrderCartCacheKey = "FastOrderCart_{0}"; // {0} будет заменено на UserId

        public CatalogModel(IApiService apiService, IMemoryCache memoryCache)
        {
            _apiService = apiService;
            _memoryCache = memoryCache;
        }

        public int StockId { get; set; }
        public string StockName { get; set; }
        public List<CatalogProductViewModel> Products { get; set; }
        public CartViewModel Cart { get; set; }

        public async Task OnGetAsync(int stockId)
        {
            StockId = stockId;
            
            // Получаем информацию о складе
            var stock = await _apiService.GetAsync<StockDto>($"api/stocks/{stockId}");
            StockName = stock.Name;

            // Получаем список товаров из API
            var products = await _apiService.GetAsync<List<ProductDto>>("api/products");
            
            // Получаем количество товаров на складе
            var stockProducts = await _apiService.GetAsync<List<StockProductDto>>($"api/stocks/{stockId}/products");
            
            // Получаем корзину пользователя
            var cart = GetOrCreateCart();
            cart.StockId = stockId;
            
            // Преобразуем DTO в ViewModel и устанавливаем количество из корзины и максимальное количество со склада
            var productViewModels = products.Select(p => new CatalogProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Description = p.Description,
                Barcode = p.Barcode,
                Price = p.Price,
                Quantity = cart.Items.FirstOrDefault(x => x.ProductId == p.Id)?.Quantity ?? 0,
                MaxQuantity = stockProducts.FirstOrDefault(sp => sp.ProductId == p.Id)?.Quantity ?? 0
            }).ToList();

            Products = productViewModels;
            Cart = cart;
        }

        private CartViewModel GetOrCreateCart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cacheKey = string.Format(FastOrderCartCacheKey, userId);
            
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
            var cacheKey = string.Format(FastOrderCartCacheKey, userId);
            
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
                    Description = product.Description,
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
            var cacheKey = string.Format(FastOrderCartCacheKey, userId);
            
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
} 