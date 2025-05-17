using Frontend.Models.Orders;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Application.DTOs;
using Application.Models;
using Frontend.Models.Catalog;
using Frontend.Services.Account;

namespace Frontend.Pages.Manager.FastOrder
{
    [Authorize(Policy = "RequireManagerRole")]
    public class CreateModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IApiService _apiService;
        
        private const string FastOrderCartCacheKey = "FastOrderCart_{0}";

        public CreateModel(IMemoryCache memoryCache, IApiService apiService)
        {
            _memoryCache = memoryCache;
            _apiService = apiService;
        }

        [BindProperty(SupportsGet = true)]
        public int StockId { get; set; }

        [BindProperty]
        public ContactInfoViewModel Contact { get; set; }

        [BindProperty]
        public PaymentViewModel Payment { get; set; }

        public CartViewModel Cart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = GetOrCreateCart();
            if (Cart.Items.Count == 0)
            {
                return RedirectToPage("/Manager/FastOrder/Catalog", new { stockId = StockId });
            }

            // Заполняем контактную информацию по умолчанию
            Contact = new ContactInfoViewModel
            {
                FirstName = "Неизвестный",
                LastName = "Неизвестный",
                Phone = "+375000000000"
            };

            // Устанавливаем сумму оплаты
            Payment = new PaymentViewModel
            {
                Amount = Cart.TotalPrice
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Cart = GetOrCreateCart();
                return Page();
            }

            try
            {
                Cart = GetOrCreateCart();
                
                if (Cart.Items.Count == 0)
                {
                    TempData["error"] = "Корзина пуста. Пожалуйста, добавьте товары в корзину.";
                    return Page();
                }

                // Создаем DTO для быстрого заказа
                var fastOrderDto = new FastOrderDto
                {
                    StockId = StockId,
                    ClientName = $"{Contact.FirstName} {Contact.LastName}",
                    ContactPhone = Contact.Phone,
                    PaymentType = Payment.PaymentType,
                    Products = Cart.Items.Select(item => new CreateOrderProductDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Name,
                        ProductDescription = item.Description,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList()
                };

                // Отправляем запрос на создание быстрого заказа
                var order = await _apiService.PostAsync<OrderDto, FastOrderDto>("api/orders/fast", fastOrderDto);

                // Очищаем корзину
                ClearCart();

                return RedirectToPage("/Manager/Order/Manage", new { id = order.Id });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Произошла ошибка при оформлении заказа. Пожалуйста, попробуйте позже.";
                
                Cart = GetOrCreateCart();
                return Page();
            }
        }

        private CartViewModel GetOrCreateCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        private void ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cacheKey = string.Format(FastOrderCartCacheKey, userId);
            
            var cart = new CartViewModel();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));
            _memoryCache.Set(cacheKey, cart, cacheOptions);
        }
    }
} 