using Application.Data.Repository;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireManagerRole")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsStore _productsStore;

        public ProductsController(IProductsStore productsStore)
        {
            _productsStore = productsStore;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productsStore.GetAllAsync();
            return Ok(products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockProducts = p.StockProducts?.Select(sp => new StockProductDto
                {
                    Id = sp.Id,
                    StockId = sp.StockId,
                    ProductId = sp.ProductId,
                    Quantity = sp.Quantity,
                    Price = sp.Product?.Price ?? 0
                }).ToList()
            }));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productsStore.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockProducts = product.StockProducts?.Select(sp => new StockProductDto
                {
                    Id = sp.Id,
                    StockId = sp.StockId,
                    ProductId = sp.ProductId,
                    Quantity = sp.Quantity,
                    Price = sp.Product?.Price ?? 0
                }).ToList()
            });
        }

        // POST: api/Products
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price
            };

            await _productsStore.Save(product);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price
                });
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productsStore.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;

            await _productsStore.Save(product);

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productsStore.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productsStore.Delete(id);

            return NoContent();
        }
    }
} 