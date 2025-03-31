using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Model.Sales;
using Application.Services;

namespace Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesStore _salesStore;

        public SalesController(ISalesStore salesStore)
        {
            _salesStore = salesStore;
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
                    SaleType = s.SaleType
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
                SaleType = sale.SaleType
            };

            return Ok(saleDto);
        }

        [HttpPost]
        public ActionResult<SaleDto> CreateSale(CreateSaleDto createSaleDto)
        {
            var sale = new Sale
            {
                OrderId = createSaleDto.OrderId,
                StockId = createSaleDto.StockId,
                SaleDate = DateTime.Now,
                SaleType = createSaleDto.SaleType
            };

            _salesStore.Save(sale);
            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSale(int id, UpdateSaleDto updateSaleDto)
        {
            var sale = _salesStore.Get(id);
            if (sale == null)
                return NotFound();

            sale.OrderId = updateSaleDto.OrderId;
            sale.StockId = updateSaleDto.StockId;
            sale.SaleType = updateSaleDto.SaleType;

            _salesStore.Save(sale);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSale(int id)
        {
            var sale = _salesStore.Get(id);
            if (sale == null)
                return NotFound();

            _salesStore.Delete(id);
            return NoContent();
        }
    }
} 