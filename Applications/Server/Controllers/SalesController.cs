using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Model.Sales;
using Application.Services;
using Application.Model.Orders;
using Application.Model.Stocks;

namespace Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesStore _salesStore;
        private readonly IOrderStore _orderStore;
        private readonly IStockStore _stockStore;
        private readonly IStockProductsStore _stockProductsStore;

        public SalesController(
            ISalesStore salesStore,
            IOrderStore orderStore,
            IStockStore stockStore,
            IStockProductsStore stockProductsStore)
        {
            _salesStore = salesStore;
            _orderStore = orderStore;
            _stockStore = stockStore;
            _stockProductsStore = stockProductsStore;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SaleDto>> GetSales()
        {
            var sales = _salesStore.All
                .Select(s => new SaleDto
                {
                    Id = s.Id,
                    OrderId = s.OrderId,
                    OrderNumber = s.Order.OrderNumber,
                    StockId = s.StockId,
                    StockName = s.Stock.Name,
                    SaleDate = s.SaleDate,
                    TotalAmount = s.TotalAmount,
                    Status = s.Status,
                    StatusDisplay = s.Status.ToString(),
                    ClientName = s.Order.ClientName,
                    ClientPhone = s.Order.ContactPhone
                })
                .ToList();

            return Ok(sales);
        }

        [HttpGet("{id}")]
        public ActionResult<SaleDto> GetSale(int id)
        {
            var sale = _salesStore.Get(id);
            if (sale == null)
                return NotFound();

            var saleDto = new SaleDto
            {
                Id = sale.Id,
                OrderId = sale.OrderId,
                OrderNumber = sale.Order.OrderNumber,
                StockId = sale.StockId,
                StockName = sale.Stock.Name,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                Status = sale.Status,
                StatusDisplay = sale.Status.ToString(),
                ClientName = sale.Order.ClientName,
                ClientPhone = sale.Order.ContactPhone
            };

            return Ok(saleDto);
        }

        [HttpPost]
        public ActionResult<SaleDto> CreateSale(CreateSaleDto createSaleDto)
        {
            var order = _orderStore.Get(createSaleDto.OrderId);
            if (order == null)
                return NotFound("Заказ не найден");

            if (order.State != Order.States.Completed)
                return BadRequest("Заказ должен быть завершен");

            var stock = _stockStore.Get(createSaleDto.StockId);
            if (stock == null)
                return NotFound("Склад не найден");

            // Проверяем наличие товаров на складе
            foreach (var orderProduct in order.Products)
            {
                var stockProduct = _stockProductsStore.All
                    .FirstOrDefault(sp => sp.StockId == createSaleDto.StockId && sp.ProductId == orderProduct.ProductId);

                if (stockProduct == null || stockProduct.Quantity < orderProduct.Quantity)
                    return BadRequest($"Недостаточно товара {orderProduct.Product.Name} на складе");
            }

            var sale = new Sale
            {
                OrderId = createSaleDto.OrderId,
                StockId = createSaleDto.StockId,
                SaleDate = DateTime.Now,
                Status = SaleStatus.Processing,
                TotalAmount = order.Products.Sum(op => op.ProductPrice * op.Quantity)
            };

            _salesStore.Save(sale);

            // Обновляем количество товаров на складе
            foreach (var orderProduct in order.Products)
            {
                var stockProduct = _stockProductsStore.All
                    .First(sp => sp.StockId == createSaleDto.StockId && sp.ProductId == orderProduct.ProductId);

                stockProduct.Quantity -= orderProduct.Quantity;
                _stockProductsStore.Save(stockProduct);
            }

            // Обновляем статус продажи
            sale.Status = SaleStatus.Completed;
            _salesStore.Save(sale);

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSale(int id, UpdateSaleDto updateSaleDto)
        {
            var sale = _salesStore.Get(id);
            if (sale == null)
                return NotFound();

            if (sale.Status == SaleStatus.Cancelled)
                return BadRequest("Нельзя изменить отмененную продажу");

            sale.OrderId = updateSaleDto.OrderId;
            sale.StockId = updateSaleDto.StockId;
            sale.Status = updateSaleDto.Status;

            _salesStore.Save(sale);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSale(int id)
        {
            var sale = _salesStore.Get(id);
            if (sale == null)
                return NotFound();

            if (sale.Status == SaleStatus.Completed)
            {
                // Возвращаем товары на склад
                foreach (var orderProduct in sale.Order.Products)
                {
                    var stockProduct = _stockProductsStore.All
                        .FirstOrDefault(sp => sp.StockId == sale.StockId && sp.ProductId == orderProduct.ProductId);

                    if (stockProduct != null)
                    {
                        stockProduct.Quantity += orderProduct.Quantity;
                        _stockProductsStore.Save(stockProduct);
                    }
                }
            }

            _salesStore.Delete(id);
            return NoContent();
        }
    }
} 