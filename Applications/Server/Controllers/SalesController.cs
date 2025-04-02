using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Sales;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetAllSales()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetSaleById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<SaleDto>> CreateSale(CreateSaleDto createSaleDto)
        {
            var sale = await _saleService.CreateSaleAsync(createSaleDto);
            return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SaleDto>> UpdateSale(int id, UpdateSaleDto updateSaleDto)
        {
            try
            {
                var sale = await _saleService.UpdateSaleAsync(id, updateSaleDto);
                return Ok(sale);
            }
            catch (BusinessException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            try
            {
                await _saleService.DeleteSaleAsync(id);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSalesByOrderId(int orderId)
        {
            var sales = await _saleService.GetSalesByOrderIdAsync(orderId);
            return Ok(sales);
        }

        [HttpGet("stock/{stockId}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSalesByStockId(int stockId)
        {
            var sales = await _saleService.GetSalesByStockIdAsync(stockId);
            return Ok(sales);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSalesByStatus(SaleStatus status)
        {
            var sales = await _saleService.GetSalesByStatusAsync(status);
            return Ok(sales);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSalesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
            return Ok(sales);
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult<SaleDto>> CompleteSale(int id)
        {
            try
            {
                var sale = await _saleService.CompleteSaleAsync(id);
                return Ok(sale);
            }
            catch (BusinessException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<SaleDto>> CancelSale(int id)
        {
            try
            {
                var sale = await _saleService.CancelSaleAsync(id);
                return Ok(sale);
            }
            catch (BusinessException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}