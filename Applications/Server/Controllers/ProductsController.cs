using System.ComponentModel.DataAnnotations;
using Application.Data.Repository;
using Application.DTOs;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Services.Products;

namespace Application.Controllers
{
    /// <summary>
    /// Контроллер для управления товарами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdminRole")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Получить список всех товаров
        /// </summary>
        /// <returns>Список товаров</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Получить товар по ID
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>Информация о товаре</returns>
        /// <response code="404">Товар не найден</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Товар с ID {id} не найден", ex);
            }
        }

        /// <summary>
        /// Создать новый товар
        /// </summary>
        /// <param name="createProductDto">Данные для создания товара</param>
        /// <returns>Созданный товар</returns>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные товара", ModelState);
            }

            var product = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        /// <summary>
        /// Обновить существующий товар
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="updateProductDto">Данные для обновления товара</param>
        /// <returns>Обновленный товар</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Товар не найден</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные товара", ModelState);
            }

            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Товар с ID {id} не найден", ex);
            }
        }

        /// <summary>
        /// Удалить товар
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>Результат операции</returns>
        /// <response code="404">Товар не найден</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Товар с ID {id} не найден", ex);
            }
        }
    }
} 