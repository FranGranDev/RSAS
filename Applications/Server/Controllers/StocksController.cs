using Application.Data.Repository;
using Application.DTOs;
using Application.Model.Stocks;
using Application.Services.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireManagerRole")]
    public class StocksController : ControllerBase
    {
        private readonly IStockStore _stockStore;

        public StocksController(IStockStore stockStore)
        {
            _stockStore = stockStore;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStocks()
        {
            var stocks = await _stockStore.GetAllStocksAsync();
            return Ok(stocks.Select(s => new StockDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                Capacity = s.Capacity
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockDto>> GetStock(int id)
        {
            var stock = await _stockStore.GetStockByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(new StockDto
            {
                Id = stock.Id,
                Name = stock.Name,
                Address = stock.Address,
                Capacity = stock.Capacity
            });
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<StockDto>> CreateStock(CreateStockDto createStockDto)
        {
            var stock = new Stock
            {
                Name = createStockDto.Name,
                Address = createStockDto.Address,
                Capacity = createStockDto.Capacity
            };

            await _stockStore.CreateStockAsync(stock);

            return CreatedAtAction(nameof(GetStock), new { id = stock.Id }, new StockDto
            {
                Id = stock.Id,
                Name = stock.Name,
                Address = stock.Address,
                Capacity = stock.Capacity
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateStock(int id, UpdateStockDto updateStockDto)
        {
            var stock = await _stockStore.GetStockByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            stock.Name = updateStockDto.Name;
            stock.Address = updateStockDto.Address;
            stock.Capacity = updateStockDto.Capacity;

            await _stockStore.UpdateStockAsync(stock);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _stockStore.GetStockByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            await _stockStore.DeleteStockAsync(id);

            return NoContent();
        }
    }
} 