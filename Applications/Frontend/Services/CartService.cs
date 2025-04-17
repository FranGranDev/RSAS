using Frontend.Models.Catalog;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

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

        public CartService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public CartViewModel GetOrCreateCart(string userId)
        {
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

        public void UpdateCart(string userId, CartViewModel cart)
        {
            var cacheKey = string.Format(CartCacheKey, userId);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));
            _memoryCache.Set(cacheKey, cart, cacheOptions);
        }

        public void ClearCart(string userId)
        {
            var cacheKey = string.Format(CartCacheKey, userId);
            var cart = new CartViewModel();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));
            _memoryCache.Set(cacheKey, cart, cacheOptions);
        }
    }
} 