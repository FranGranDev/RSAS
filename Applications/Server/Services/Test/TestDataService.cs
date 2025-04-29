using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Data;
using Application.DTOs;
using Application.DTOs.Test;
using Application.Exceptions;
using Application.Models;
using Application.Services;
using Application.Services.Stocks;
using Application.Utils;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Test
{
    public class TestDataService : ITestDataService
    {
        private readonly IOrderService _orderService;
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private readonly Random _random;

        public TestDataService(
            IOrderService orderService,
            IStockService stockService,
            IProductService productService,
            AppDbContext context)
        {
            _orderService = orderService;
            _stockService = stockService;
            _productService = productService;
            _context = context;
            _random = new Random();
        }

        public async Task GenerateTestSalesAsync(GenerateSalesDto dto, string userId)
        {
            if (dto.StartDate >= dto.EndDate)
            {
                throw new ValidationException("Дата начала периода должна быть меньше даты окончания");
            }

            var totalDays = (dto.EndDate - dto.StartDate).TotalDays;
            var products = await _context.Products.ToListAsync();
            var stocks = await _context.Stocks.ToListAsync();

            if (!products.Any())
            {
                throw new ValidationException("В системе нет товаров");
            }

            if (!stocks.Any())
            {
                throw new ValidationException("В системе нет складов");
            }

            for (int i = 0; i < dto.SalesCount; i++)
            {
                // Генерируем случайную дату в пределах периода
                var randomDays = _random.NextDouble() * totalDays;
                var orderDate = dto.StartDate.AddDays(randomDays);
                
                try
                {
                    // Создаем и выполняем заказ
                    await CreateTestOrderAsync(products, stocks, userId, orderDate, dto);
                }
                finally
                {
                    // Сбрасываем системное время
                    SystemTime.Reset();
                }
            }
        }

        private async Task CreateTestOrderAsync(List<Product> products, List<Stock> stocks, string userId, DateTime orderDate, GenerateSalesDto dto)
        {
            // Устанавливаем время создания заказа
            SystemTime.SetCustomTime(orderDate);

            // Выбираем случайный склад
            var stock = stocks[_random.Next(stocks.Count)];
            
            // Выбираем случайное количество товаров в пределах заданного диапазона
            var productsCount = _random.Next(dto.MinProductsPerOrder, dto.MaxProductsPerOrder + 1);
            var selectedProducts = products.OrderBy(x => _random.Next()).Take(productsCount).ToList();
            
            // Создаем список товаров для заказа
            var orderProducts = new List<CreateOrderProductDto>();
            foreach (var product in selectedProducts)
            {
                // Генерируем случайное количество в пределах заданного диапазона
                var quantity = _random.Next(dto.MinProductQuantity, dto.MaxProductQuantity + 1);
                
                // Добавляем товары на склад
                await _stockService.AddProductToStockAsync(stock.Id, product.Id, quantity);
                
                orderProducts.Add(new CreateOrderProductDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            // Создаем заказ
            var createOrderDto = new CreateOrderDto
            {
                StockId = stock.Id,
                ClientName = $"Test Client {_random.Next(1000)}",
                ContactPhone = $"+37529{_random.Next(1000000, 9999999)}",
                PaymentType = Order.PaymentTypes.Cash,
                Products = orderProducts,
                Delivery = new CreateDeliveryDto
                {
                    DeliveryDate = orderDate.AddDays(_random.Next(dto.MinDeliveryDays, dto.MaxDeliveryDays + 1)),
                    City = "Минск",
                    Street = $"Тестовая улица {_random.Next(1, 100)}",
                    House = _random.Next(1, 100).ToString(),
                    Flat = _random.Next(1, 200).ToString(),
                    PostalCode = $"{_random.Next(100000, 999999)}"
                }
            };

            var order = await _orderService.CreateOrderAsync(createOrderDto, userId);

            // Ждем случайное время для выполнения заказа
            var executionTime = orderDate.AddMinutes(_random.Next(1, dto.AverageOrderDurationMinutes / 2));
            SystemTime.SetCustomTime(executionTime);
            
            // Выполняем заказ
            await _orderService.ExecuteOrderAsync(order.Id, order.StockId.Value);
            
            // Ждем еще немного для завершения заказа
            var completionTime = executionTime.AddMinutes(_random.Next(1, dto.AverageOrderDurationMinutes / 2));
            SystemTime.SetCustomTime(completionTime);
            
            // Завершаем заказ
            await _orderService.CompleteOrderAsync(order.Id);
        }
    }
} 