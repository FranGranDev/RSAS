using Application.DTOs;
using Application.Exceptions;
using Application.Services.Stocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    /// <summary>
    ///     Контроллер для управления складами
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        /// <summary>
        ///     Получить список всех складов
        /// </summary>
        /// <returns>Список складов</returns>
        /// <response code="403">Недостаточно прав для просмотра всех складов</response>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStocks()
        {
            var stocks = await _stockService.GetAllStocksAsync();
            return Ok(stocks);
        }

        /// <summary>
        ///     Получить склад по ID
        /// </summary>
        /// <param name="id">ID склада</param>
        /// <returns>Информация о складе</returns>
        /// <response code="403">Недостаточно прав для просмотра склада</response>
        /// <response code="404">Склад не найден</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockDto>> GetStock(int id)
        {
            try
            {
                var stock = await _stockService.GetStockByIdAsync(id);
                return Ok(stock);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {id} не найден");
            }
            catch (BusinessException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        ///     Создать новый склад
        /// </summary>
        /// <param name="createStockDto">Данные для создания склада</param>
        /// <returns>Созданный склад</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для создания склада</response>
        [HttpPost]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockDto>> CreateStock(CreateStockDto createStockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var stock = await _stockService.CreateStockAsync(createStockDto);
                return CreatedAtAction(nameof(GetStock), new { id = stock.Id }, stock);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании склада: {ex.Message}");
            }
        }

        /// <summary>
        ///     Обновить информацию о складе
        /// </summary>
        /// <param name="id">ID склада</param>
        /// <param name="updateStockDto">Данные для обновления склада</param>
        /// <returns>Обновленный склад</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для обновления склада</response>
        /// <response code="404">Склад не найден</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockDto>> UpdateStock(int id, UpdateStockDto updateStockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var stock = await _stockService.UpdateStockAsync(id, updateStockDto);
                return Ok(stock);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении склада: {ex.Message}");
            }
        }

        /// <summary>
        ///     Удалить склад
        /// </summary>
        /// <param name="id">ID склада</param>
        /// <returns>Результат операции</returns>
        /// <response code="403">Недостаточно прав для удаления склада</response>
        /// <response code="404">Склад не найден</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            try
            {
                await _stockService.DeleteStockAsync(id);
                return NoContent();
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {id} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении склада: {ex.Message}");
            }
        }

        /// <summary>
        ///     Получить склад по названию
        /// </summary>
        /// <param name="name">Название склада</param>
        /// <returns>Информация о складе</returns>
        /// <response code="403">Недостаточно прав для просмотра склада</response>
        /// <response code="404">Склад не найден</response>
        [HttpGet("name/{name}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockDto>> GetStockByName(string name)
        {
            try
            {
                var stock = await _stockService.GetStockByNameAsync(name);
                return Ok(stock);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с названием {name} не найден");
            }
        }

        /// <summary>
        ///     Получить склад по адресу
        /// </summary>
        /// <param name="address">Адрес склада</param>
        /// <returns>Информация о складе</returns>
        /// <response code="403">Недостаточно прав для просмотра склада</response>
        /// <response code="404">Склад не найден</response>
        [HttpGet("address/{address}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockDto>> GetStockByAddress(string address)
        {
            try
            {
                var stock = await _stockService.GetStockByAddressAsync(address);
                return Ok(stock);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад по адресу {address} не найден");
            }
        }

        /// <summary>
        ///     Получить склады по городу
        /// </summary>
        /// <param name="city">Город</param>
        /// <returns>Список складов</returns>
        /// <response code="403">Недостаточно прав для просмотра складов</response>
        [HttpGet("city/{city}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStocksByCity(string city)
        {
            var stocks = await _stockService.GetStocksByCityAsync(city);
            return Ok(stocks);
        }

        /// <summary>
        ///     Проверить существование склада по названию
        /// </summary>
        /// <param name="name">Название склада</param>
        /// <returns>Результат проверки</returns>
        /// <response code="403">Недостаточно прав для проверки</response>
        [HttpGet("exists/name/{name}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<bool>> ExistsByName(string name)
        {
            var exists = await _stockService.ExistsByNameAsync(name);
            return Ok(exists);
        }

        /// <summary>
        ///     Проверить существование склада по адресу
        /// </summary>
        /// <param name="address">Адрес склада</param>
        /// <returns>Результат проверки</returns>
        /// <response code="403">Недостаточно прав для проверки</response>
        [HttpGet("exists/address/{address}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<bool>> ExistsByAddress(string address)
        {
            var exists = await _stockService.ExistsByAddressAsync(address);
            return Ok(exists);
        }

        /// <summary>
        ///     Получить товары на складе
        /// </summary>
        /// <param name="stockId">ID склада</param>
        /// <returns>Список товаров на складе</returns>
        /// <response code="403">Недостаточно прав для просмотра товаров</response>
        /// <response code="404">Склад не найден</response>
        [HttpGet("{stockId}/products")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<StockProductDto>>> GetStockProducts(int stockId)
        {
            try
            {
                var products = await _stockService.GetStockProductsAsync(stockId);
                return Ok(products);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {stockId} не найден");
            }
        }

        /// <summary>
        ///     Обновить количество товара на складе
        /// </summary>
        /// <param name="stockId">ID склада</param>
        /// <param name="productId">ID товара</param>
        /// <param name="quantity">Новое количество</param>
        /// <returns>Обновленная информация о товаре на складе</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для обновления количества</response>
        /// <response code="404">Склад или товар не найден</response>
        [HttpPut("{stockId}/products/{productId}/quantity")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockProductDto>> UpdateStockProductQuantity(
            int stockId,
            int productId,
            [FromQuery] int quantity)
        {
            if (quantity < 0)
            {
                return BadRequest("Количество товара не может быть отрицательным");
            }

            try
            {
                var stockProduct = await _stockService.UpdateStockProductQuantityAsync(stockId, productId, quantity);
                return Ok(stockProduct);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {stockId} не найден");
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении количества товара: {ex.Message}");
            }
        }

        /// <summary>
        ///     Добавить товар на склад
        /// </summary>
        /// <param name="stockId">ID склада</param>
        /// <param name="productId">ID товара</param>
        /// <param name="quantity">Количество товара</param>
        /// <returns>Информация о товаре на складе</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для добавления товара</response>
        /// <response code="404">Склад или товар не найден</response>
        [HttpPost("{stockId}/products/{productId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockProductDto>> AddProductToStock(
            int stockId,
            int productId,
            [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Количество товара должно быть больше 0");
            }

            try
            {
                var stockProduct = await _stockService.AddProductToStockAsync(stockId, productId, quantity);
                return CreatedAtAction(nameof(GetStockProduct), new { stockId, productId }, stockProduct);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {stockId} не найден");
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при добавлении товара на склад: {ex.Message}");
            }
        }

        /// <summary>
        ///     Удалить товар со склада
        /// </summary>
        /// <param name="stockId">ID склада</param>
        /// <param name="productId">ID товара</param>
        /// <returns>Результат операции</returns>
        /// <response code="403">Недостаточно прав для удаления товара</response>
        /// <response code="404">Склад или товар не найден</response>
        [HttpDelete("{stockId}/products/{productId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> RemoveProductFromStock(int stockId, int productId)
        {
            try
            {
                await _stockService.RemoveProductFromStockAsync(stockId, productId);
                return NoContent();
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {stockId} не найден");
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении товара со склада: {ex.Message}");
            }
        }

        /// <summary>
        ///     Получить информацию о товаре на складе
        /// </summary>
        /// <param name="stockId">ID склада</param>
        /// <param name="productId">ID товара</param>
        /// <returns>Информация о товаре на складе</returns>
        /// <response code="403">Недостаточно прав для просмотра товара</response>
        /// <response code="404">Склад или товар не найден</response>
        [HttpGet("{stockId}/products/{productId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<StockProductDto>> GetStockProduct(int stockId, int productId)
        {
            try
            {
                var stockProduct = await _stockService.GetStockProductAsync(stockId, productId);
                return Ok(stockProduct);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {stockId} не найден");
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
        }

        /// <summary>
        ///     Проверить наличие товара на складе
        /// </summary>
        /// <param name="stockId">ID склада</param>
        /// <param name="productId">ID товара</param>
        /// <returns>Результат проверки</returns>
        /// <response code="403">Недостаточно прав для проверки</response>
        /// <response code="404">Склад или товар не найден</response>
        [HttpGet("{stockId}/products/{productId}/exists")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<bool>> HasProductOnStock(int stockId, int productId)
        {
            try
            {
                var exists = await _stockService.HasProductOnStockAsync(stockId, productId);
                return Ok(exists);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {stockId} не найден");
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
        }

        /// <summary>
        ///     Получить количество товара на складе
        /// </summary>
        /// <param name="stockId">ID склада</param>
        /// <param name="productId">ID товара</param>
        /// <returns>Количество товара</returns>
        /// <response code="403">Недостаточно прав для просмотра количества</response>
        /// <response code="404">Склад или товар не найден</response>
        [HttpGet("{stockId}/products/{productId}/quantity")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<int>> GetProductQuantityOnStock(int stockId, int productId)
        {
            try
            {
                var quantity = await _stockService.GetProductQuantityOnStockAsync(stockId, productId);
                return Ok(quantity);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {stockId} не найден");
            }
            catch (ProductNotFoundException)
            {
                return NotFound($"Товар с ID {productId} не найден");
            }
        }
    }
}