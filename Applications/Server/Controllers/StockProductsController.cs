using Application.Data.Repository;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireManagerRole")]
    public class StockProductsController : ControllerBase
    {
        private readonly IStockProductsStore _stockProductsStore;

        public StockProductsController(IStockProductsStore stockProductsStore)
        {
            _stockProductsStore = stockProductsStore;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockProductDto>>> GetStockProducts()
        {
            var stockProducts = await _stockProductsStore.GetAllStockProductsAsync();
            return Ok(stockProducts.Select(sp => new StockProductDto
            {
                Id = sp.Id,
                StockId = sp.StockId,
                ProductId = sp.ProductId,
                Quantity = sp.Quantity,
                LastUpdated = sp.LastUpdated
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockProductDto>> GetStockProduct(int id)
        {
            var stockProduct = await _stockProductsStore.GetStockProductByIdAsync(id);
            if (stockProduct == null)
            {
                return NotFound();
            }

            return Ok(new StockProductDto
            {
                Id = stockProduct.Id,
                StockId = stockProduct.StockId,
                ProductId = stockProduct.ProductId,
                Quantity = stockProduct.Quantity,
                LastUpdated = stockProduct.LastUpdated
            });
        }

        [HttpGet("stock/{stockId}")]
        public async Task<ActionResult<IEnumerable<StockProductDto>>> GetStockProductsByStock(int stockId)
        {
            var stockProducts = await _stockProductsStore.GetStockProductsByStockIdAsync(stockId);
            return Ok(stockProducts.Select(sp => new StockProductDto
            {
                Id = sp.Id,
                StockId = sp.StockId,
                ProductId = sp.ProductId,
                Quantity = sp.Quantity,
                LastUpdated = sp.LastUpdated
            }));
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<StockProductDto>> CreateStockProduct(CreateStockProductDto createStockProductDto)
        {
            var stockProduct = new StockProduct
            {
                StockId = createStockProductDto.StockId,
                ProductId = createStockProductDto.ProductId,
                Quantity = createStockProductDto.Quantity,
                LastUpdated = DateTime.UtcNow
            };

            await _stockProductsStore.CreateStockProductAsync(stockProduct);

            return CreatedAtAction(nameof(GetStockProduct), new { id = stockProduct.Id }, new StockProductDto
            {
                Id = stockProduct.Id,
                StockId = stockProduct.StockId,
                ProductId = stockProduct.ProductId,
                Quantity = stockProduct.Quantity,
                LastUpdated = stockProduct.LastUpdated
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateStockProduct(int id, UpdateStockProductDto updateStockProductDto)
        {
            var stockProduct = await _stockProductsStore.GetStockProductByIdAsync(id);
            if (stockProduct == null)
            {
                return NotFound();
            }

            stockProduct.StockId = updateStockProductDto.StockId;
            stockProduct.ProductId = updateStockProductDto.ProductId;
            stockProduct.Quantity = updateStockProductDto.Quantity;
            stockProduct.LastUpdated = DateTime.UtcNow;

            await _stockProductsStore.UpdateStockProductAsync(stockProduct);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteStockProduct(int id)
        {
            var stockProduct = await _stockProductsStore.GetStockProductByIdAsync(id);
            if (stockProduct == null)
            {
                return NotFound();
            }

            await _stockProductsStore.DeleteStockProductAsync(id);

            return NoContent();
        }
    }
} 