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
using System.Text.Json;

namespace Frontend.Pages.Client.Order
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IClientService _clientService;
        private readonly IMemoryCache _memoryCache;
        private readonly IApiService _apiService;
        
        private const string CartCacheKey = "Cart_{0}";

        public CreateModel(IMemoryCache memoryCache, IApiService apiService, IClientService clientService)
        {
            _clientService = clientService;
            _memoryCache = memoryCache;
            _apiService = apiService;
        }

        [BindProperty]
        public ContactInfoViewModel Contact { get; set; }

        [BindProperty]
        public DeliveryViewModel Delivery { get; set; }

        [BindProperty]
        public PaymentViewModel Payment { get; set; }

        public CartViewModel Cart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = GetOrCreateCart();
            if (Cart.Items.Count == 0)
            {
                return RedirectToPage("/Client/Catalog/Index");
            }

            // Получаем информацию о пользователе
            var client = await _clientService.GetCurrentClientAsync();

            // Заполняем контактную информацию
            Contact = new ContactInfoViewModel
            {
                FirstName = client?.FirstName ?? string.Empty,
                LastName = client?.LastName ?? string.Empty,
                Phone = client?.Phone ?? string.Empty,
            };

            // Устанавливаем дату доставки на завтра
            Delivery = new DeliveryViewModel
            {
                DeliveryDate = DateTime.Now.AddDays(1)
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

                // Создаем DTO для заказа
                var createOrderDto = new CreateOrderDto
                {
                    StockId = null, // Пока не используем склад
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
                    }).ToList(),
                    Delivery = new CreateDeliveryDto
                    {
                        DeliveryDate = Delivery.DeliveryDate,
                        City = Delivery.City,
                        Street = Delivery.Street,
                        House = Delivery.House,
                        Flat = Delivery.Flat,
                        PostalCode = Delivery.PostalCode
                    }
                };

                // Отправляем запрос на создание заказа
                var order = await _apiService.PostAsync<OrderDto, CreateOrderDto>("api/orders", createOrderDto);

                // Очищаем корзину
                ClearCart();

                return RedirectToPage("/Client/Order/Success", new { orderId = order.Id });
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

        private void ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cacheKey = string.Format(CartCacheKey, userId);
            
            var cart = new CartViewModel();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));
            _memoryCache.Set(cacheKey, cart, cacheOptions);
        }
    }

    // Добавим класс для десериализации ошибок валидации
    public class ValidationErrorResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }
    }
} 