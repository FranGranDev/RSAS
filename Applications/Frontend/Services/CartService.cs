using Frontend.Models.Catalog;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Frontend.Services
{
    public interface ICartService
    {
        CartViewModel GetOrCreateCart(string userId);
        void UpdateCart(string userId, CartViewModel cart);
        void ClearCart(string userId);
    }

    public class CartService : ICartService
    {
        private readonly IMemoryCache _memoryCache;
        private const string CartCacheKey = "Cart_{0}"; // {0} будет заменено на UserId
        private readonly ILogger<CartService> _logger;

        public CartService(IMemoryCache memoryCache, ILogger<CartService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public CartViewModel GetOrCreateCart(string userId)
        {
            var cacheKey = string.Format(CartCacheKey, userId);
            
            if (!_memoryCache.TryGetValue(cacheKey, out CartViewModel cart))
            {
                _logger.LogInformation("Создана новая корзина для пользователя {UserId}", userId);
                cart = new CartViewModel();
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(7));
                _memoryCache.Set(cacheKey, cart, cacheOptions);
            }
            else
            {
                _logger.LogInformation("Получена существующая корзина для пользователя {UserId}. Количество товаров: {Count}", 
                    userId, cart.Items.Count);
            }
            
            return cart;
        }

        public void UpdateCart(string userId, CartViewModel cart)
        {
            var cacheKey = string.Format(CartCacheKey, userId);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));
            _memoryCache.Set(cacheKey, cart, cacheOptions);
            _logger.LogInformation("Корзина пользователя {UserId} обновлена. Количество товаров: {Count}", 
                userId, cart.Items.Count);
        }

        public void ClearCart(string userId)
        {
            var cacheKey = string.Format(CartCacheKey, userId);
            _memoryCache.Remove(cacheKey);
            _logger.LogInformation("Корзина пользователя {UserId} очищена", userId);
        }
    }
} 