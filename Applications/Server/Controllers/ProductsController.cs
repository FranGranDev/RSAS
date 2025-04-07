using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    /// <summary>
    ///     Контроллер для управления товарами
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        ///     Получить список всех товаров
        /// </summary>
        /// <returns>Список товаров</returns>
        /// <response code="403">Недостаточно прав для просмотра всех товаров</response>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        ///     Получить товар по ID
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>Информация о товаре</returns>
        /// <response code="403">Недостаточно прав для просмотра товара</response>
        /// <response code="404">Товар не найден</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {id} не найден");
            }
            catch (BusinessException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        ///     Создать новый товар
        /// </summary>
        /// <param name="createProductDto">Данные для создания товара</param>
        /// <returns>Созданный товар</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для создания товара</response>
        [HttpPost]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании товара: {ex.Message}");
            }
        }

        /// <summary>
        ///     Обновить информацию о товаре
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="updateProductDto">Данные для обновления товара</param>
        /// <returns>Обновленный товар</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для обновления товара</response>
        /// <response code="404">Товар не найден</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении товара: {ex.Message}");
            }
        }

        /// <summary>
        ///     Удалить товар
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>Результат операции</returns>
        /// <response code="403">Недостаточно прав для удаления товара</response>
        /// <response code="404">Товар не найден</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении товара: {ex.Message}");
            }
        }

        /// <summary>
        ///     Получить товар по названию
        /// </summary>
        /// <param name="name">Название товара</param>
        /// <returns>Информация о товаре</returns>
        /// <response code="403">Недостаточно прав для просмотра товара</response>
        /// <response code="404">Товар не найден</response>
        [HttpGet("name/{name}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ProductDto>> GetProductByName(string name)
        {
            try
            {
                var product = await _productService.GetProductByNameAsync(name);
                return Ok(product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с названием {name} не найден");
            }
        }

        /// <summary>
        ///     Получить товар по штрих-коду
        /// </summary>
        /// <param name="barcode">Штрих-код товара</param>
        /// <returns>Информация о товаре</returns>
        /// <response code="403">Недостаточно прав для просмотра товара</response>
        /// <response code="404">Товар не найден</response>
        [HttpGet("barcode/{barcode}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode)
        {
            try
            {
                var product = await _productService.GetProductByBarcodeAsync(barcode);
                return Ok(product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар со штрих-кодом {barcode} не найден");
            }
        }

        /// <summary>
        ///     Получить товары по категории
        /// </summary>
        /// <param name="category">Категория товара</param>
        /// <returns>Список товаров</returns>
        /// <response code="403">Недостаточно прав для просмотра товаров</response>
        [HttpGet("category/{category}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        /// <summary>
        ///     Получить товары по диапазону цен
        /// </summary>
        /// <param name="minPrice">Минимальная цена</param>
        /// <param name="maxPrice">Максимальная цена</param>
        /// <returns>Список товаров</returns>
        /// <response code="403">Недостаточно прав для просмотра товаров</response>
        [HttpGet("price-range")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice)
        {
            var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            return Ok(products);
        }

        /// <summary>
        ///     Проверить существование товара по названию
        /// </summary>
        /// <param name="name">Название товара</param>
        /// <returns>Результат проверки</returns>
        /// <response code="403">Недостаточно прав для проверки</response>
        [HttpGet("exists/name/{name}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<bool>> ExistsByName(string name)
        {
            var exists = await _productService.ExistsByNameAsync(name);
            return Ok(exists);
        }

        /// <summary>
        ///     Проверить существование товара по штрих-коду
        /// </summary>
        /// <param name="barcode">Штрих-код товара</param>
        /// <returns>Результат проверки</returns>
        /// <response code="403">Недостаточно прав для проверки</response>
        [HttpGet("exists/barcode/{barcode}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<bool>> ExistsByBarcode(string barcode)
        {
            var exists = await _productService.ExistsByBarcodeAsync(barcode);
            return Ok(exists);
        }
    }
}