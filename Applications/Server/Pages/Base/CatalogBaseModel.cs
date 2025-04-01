using Application.ViewModel.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Pages
{
    [Authorize(Roles = "Client, Company, Admin")]
    public abstract class CatalogBaseModel : CatalogProducts
    {
        protected readonly DataManager dataManager;

        public CatalogBaseModel(DataManager dataManager, IMemoryCache memoryCache) : base(memoryCache)
        {
            this.dataManager = dataManager;
        }


        public abstract List<CatalogItemViewModel> CreateProducts();

        public void UpdateProducts()
        {
            var oldProducts = CachedProducts;
            var newProducts = CreateProducts();

            foreach (var product in newProducts)
            {
                var oldProduct = oldProducts.FirstOrDefault(p => p.Id == product.Id);
                if (oldProduct != null)
                {
                    product.TakenCount = oldProduct.TakenCount;
                }
            }

            CachedProducts = newProducts;
        }


        public virtual IActionResult OnGet()
        {
            if (CachedProducts == null)
            {
                CachedProducts = CreateProducts();
            }
            else
            {
                UpdateProducts();
            }

            return Page();
        }

        public abstract IActionResult OnPostOrder();

        public IActionResult OnGetProducts()
        {
            return Partial("_CatalogPartial", CachedProducts);
        }

        public IActionResult OnGetCartInfo()
        {
            var products = CachedProducts;

            CartInfoViewModel cart = new()
            {
                Quantity = products.Select(x => x.TakenCount).Sum(),
                TotalPrice = products.Select(x => x.ProductPrice * x.TakenCount).Sum()
            };

            return Partial("_CartInfoPartial", cart);
        }

        public IActionResult OnPostClear()
        {
            CachedProducts = CreateProducts();

            return Partial("_CatalogPartial", CachedProducts);
        }

        public IActionResult OnPostSearch(string searchString)
        {
            var products = CachedProducts;

            if (!string.IsNullOrEmpty(searchString))
            {
                HttpContext.Session.SetString("search", searchString);
                products = products.Where(p => p.Name.Contains(searchString));
            }
            else
            {
                HttpContext.Session.Remove("search");
            }

            return Partial("_CatalogPartial", products);
        }

        public IActionResult OnPostSort(string sortBy, string sortOrder)
        {
            var products = CachedProducts;

            var searchString = HttpContext.Session.GetString("search") ?? "";
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }


            switch (sortBy)
            {
                case "name":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Name);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Name);
                    }

                    break;
                case "price":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.ProductPrice);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.ProductPrice);
                    }

                    break;
                case "description":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Description);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Description);
                    }

                    break;
                case "quantity":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.Quantity);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.Quantity);
                    }

                    break;
                case "taken":
                    if (sortOrder == "asc")
                    {
                        products = products.OrderBy(p => p.TakenCount);
                    }
                    else
                    {
                        products = products.OrderByDescending(p => p.TakenCount);
                    }

                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            return Partial("_CatalogPartial", products);
        }

        public IActionResult OnPostAdd(int productId, int quantity)
        {
            var products = CachedProducts;

            var product = products.FirstOrDefault(x => x.Id == productId);
            if (product != null)
            {
                product.TakenCount += quantity;
                product.TakenCount = Math.Clamp(product.TakenCount, 0, int.MaxValue);
            }

            CachedProducts = products;


            return new JsonResult(new { quantity = product.TakenCount });
        }

        public IActionResult OnPostQuantity(int productId, int quantity)
        {
            var products = CachedProducts;

            var product = products.FirstOrDefault(x => x.Id == productId);
            if (product != null)
            {
                quantity = Math.Max(quantity, 0);

                product.TakenCount = quantity;
            }

            CachedProducts = products;


            return new JsonResult(new { quantity = product.TakenCount });
        }
    }
}