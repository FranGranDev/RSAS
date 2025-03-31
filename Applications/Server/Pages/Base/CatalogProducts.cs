using Application.Services;
using Application.ViewModel.Catalog;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Pages
{
    public class CatalogProducts : PageModel
    {
        public CatalogProducts(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        private readonly IMemoryCache memoryCache;


        /// <summary>
        /// Return a copy of self
        /// </summary>
        public IEnumerable<CatalogItemViewModel> CachedProducts
        {
            get
            {
                IEnumerable<CatalogItemViewModel> products;
                if (memoryCache.TryGetValue($"{User.Identity.Name}_Products", out products))
                {
                    return products;
                }

                return null;
            }
            set
            {
                memoryCache.Set($"{User.Identity.Name}_Products", value);
            }
        }
    }
}
