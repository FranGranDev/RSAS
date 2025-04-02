using Application.DTOs;
using Application.Model.Sales;
using Application.Models;
using Application.Services.Repository;
using Application.Services.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Repository;
using Server.Services.Sales;

namespace Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleService _saleService;

        public SalesController(
            ISaleService saleService,
            IOrderRepository orderRepository,
            ISaleRepository saleRepository,
            IStockRepository stockRepository)
        {
            _saleService = saleService;
            _orderRepository = orderRepository;
            _saleRepository = saleRepository;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SaleDto>> GetSales()
        {
            var sales = _saleRepository.All
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
            var sale = _saleRepository.Get(id);
            if (sale == null)
            {
                return NotFound();
            }

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
            var order = _orderRepository.Get(createSaleDto.OrderId);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            if (order.State != Order.States.Completed)
            {
                return BadRequest("Заказ должен быть завершен");
            }

            var stock = _stockRepository.Get(createSaleDto.StockId);
            if (stock == null)
            {
                return NotFound("Склад не найден");
            }

            // Проверяем наличие товаров на складе
            foreach (var orderProduct in order.Products)
            {
                var stockProduct = _stockRepository.All
                    .FirstOrDefault(sp =>
                        sp.StockId == createSaleDto.StockId && sp.ProductId == orderProduct.ProductId);

                if (stockProduct == null || stockProduct.Quantity < orderProduct.Quantity)
                {
                    return BadRequest($"Недостаточно товара {orderProduct.Product.Name} на складе");
                }
            }

            var sale = new Sale
            {
                OrderId = createSaleDto.OrderId,
                StockId = createSaleDto.StockId,
                SaleDate = DateTime.Now,
                Status = SaleStatus.Processing,
                TotalAmount = order.Products.Sum(op => op.ProductPrice * op.Quantity)
            };

            _saleRepository.Save(sale);

            // Обновляем количество товаров на складе
            foreach (var orderProduct in order.Products)
            {
                var stockProduct = _stockRepository.All
                    .First(sp => sp.StockId == createSaleDto.StockId && sp.ProductId == orderProduct.ProductId);

                stockProduct.Quantity -= orderProduct.Quantity;
                _stockRepository.Save(stockProduct);
            }

            // Обновляем статус продажи
            sale.Status = SaleStatus.Completed;
            _saleRepository.Save(sale);

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSale(int id, UpdateSaleDto updateSaleDto)
        {
            var sale = _saleRepository.Get(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.Status == SaleStatus.Cancelled)
            {
                return BadRequest("Нельзя изменить отмененную продажу");
            }

            sale.OrderId = updateSaleDto.OrderId;
            sale.StockId = updateSaleDto.StockId;
            sale.Status = updateSaleDto.Status;

            _saleRepository.Save(sale);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSale(int id)
        {
            var sale = _saleRepository.Get(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.Status == SaleStatus.Completed)
            {
                // Возвращаем товары на склад
                foreach (var orderProduct in sale.Order.Products)
                {
                    var stockProduct = _stockRepository.All
                        .FirstOrDefault(sp => sp.StockId == sale.StockId && sp.ProductId == orderProduct.ProductId);

                    if (stockProduct != null)
                    {
                        stockProduct.Quantity += orderProduct.Quantity;
                        _stockRepository.Save(stockProduct);
                    }
                }
            }

            _saleRepository.Delete(id);
            return NoContent();
        }
    }
}