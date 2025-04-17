using System.Security.Claims;
using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    /// <summary>
    ///     Контроллер для управления заказами
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        ///     Получить список всех заказов (только для менеджеров)
        /// </summary>
        /// <returns>Список заказов</returns>
        /// <response code="403">Недостаточно прав для просмотра всех заказов</response>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        ///     Получить список заказов текущего пользователя
        /// </summary>
        /// <returns>Список заказов пользователя</returns>
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        /// <summary>
        ///     Получить заказ по ID
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Информация о заказе</returns>
        /// <response code="403">Недостаточно прав для просмотра заказа</response>
        /// <response code="404">Заказ не найден</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isManager = User.IsInRole("Manager");

                // Если пользователь не менеджер и не владелец заказа - запрещаем доступ
                if (!isManager && !await _orderService.IsOrderOwnerAsync(id, userId))
                {
                    return Forbid();
                }

                return Ok(order);
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {id} не найден");
            }
        }

        /// <summary>
        ///     Создать новый заказ
        /// </summary>
        /// <param name="createOrderDto">Данные для создания заказа</param>
        /// <returns>Созданный заказ</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Склад не найден</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createOrderDto.Products == null || !createOrderDto.Products.Any())
            {
                return BadRequest("Заказ должен содержать хотя бы один товар");
            }

            if (string.IsNullOrEmpty(createOrderDto.ClientName))
            {
                return BadRequest("Имя клиента обязательно для заполнения");
            }

            if (string.IsNullOrEmpty(createOrderDto.ContactPhone))
            {
                return BadRequest("Контактный телефон обязателен для заполнения");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Не удалось определить пользователя");
                }

                var order = await _orderService.CreateOrderAsync(createOrderDto, userId);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (StockNotFoundException)
            {
                return NotFound($"Склад с ID {createOrderDto.StockId} не найден");
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Обновить существующий заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <param name="updateOrderDto">Данные для обновления заказа</param>
        /// <returns>Обновленный заказ</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для обновления заказа</response>
        /// <response code="404">Заказ не найден</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!await _orderService.IsOrderOwnerAsync(id, userId))
            {
                return Forbid();
            }

            try
            {
                await _orderService.UpdateOrderAsync(id, updateOrderDto);
                return NoContent();
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {id} не найден");
            }
            catch (InvalidOrderStateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Удалить заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Результат операции</returns>
        /// <response code="403">Недостаточно прав для удаления заказа</response>
        /// <response code="404">Заказ не найден</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!await _orderService.IsOrderOwnerAsync(id, userId))
                {
                    return Forbid();
                }

                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {id} не найден");
            }
            catch (InvalidOrderStateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Получить заказы пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список заказов пользователя</returns>
        /// <response code="403">Недостаточно прав для просмотра заказов пользователя</response>
        [HttpGet("user/{userId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders(string userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при получении заказов пользователя: {ex.Message}");
            }
        }

        /// <summary>
        ///     Обновить информацию о доставке заказа
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <param name="updateDeliveryDto">Данные для обновления доставки</param>
        /// <returns>Обновленная информация о доставке</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для обновления доставки</response>
        /// <response code="404">Заказ не найден</response>
        [HttpPut("{orderId}/delivery")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<DeliveryDto>> UpdateDelivery(int orderId, UpdateDeliveryDto updateDeliveryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delivery = await _orderService.UpdateDeliveryAsync(orderId, updateDeliveryDto);
                return Ok(delivery);
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {orderId} не найден");
            }
            catch (InvalidOrderStateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Получить доставки по статусу
        /// </summary>
        /// <param name="status">Статус доставки</param>
        /// <returns>Список доставок</returns>
        /// <response code="403">Недостаточно прав для просмотра доставок</response>
        [HttpGet("deliveries/status/{status}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveriesByStatus(string status)
        {
            var deliveries = await _orderService.GetDeliveriesByStatusAsync(status);
            return Ok(deliveries);
        }

        /// <summary>
        ///     Получить доставки по диапазону дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <returns>Список доставок</returns>
        /// <response code="403">Недостаточно прав для просмотра доставок</response>
        [HttpGet("deliveries/date-range")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveriesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var deliveries = await _orderService.GetDeliveriesByDateRangeAsync(startDate, endDate);
            return Ok(deliveries);
        }

        /// <summary>
        ///     Выполнить заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Обновленный заказ</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для выполнения заказа</response>
        /// <response code="404">Заказ не найден</response>
        [HttpPost("{id}/execute")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<OrderDto>> ExecuteOrder(int id)
        {
            try
            {
                var order = await _orderService.ExecuteOrderAsync(id);
                return Ok(order);
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {id} не найден");
            }
            catch (StockNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Отменить заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Обновленный заказ</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для отмены заказа</response>
        /// <response code="404">Заказ не найден</response>
        [HttpPost("{id}/cancel")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<OrderDto>> CancelOrder(int id)
        {
            try
            {
                var order = await _orderService. CancelOrderAsync(id);
                return Ok(order);
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {id} не найден");
            }
            catch (InvalidOrderStateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Завершить заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Обновленный заказ</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для завершения заказа</response>
        /// <response code="404">Заказ не найден</response>
        [HttpPost("{id}/complete")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<OrderDto>> CompleteOrder(int id)
        {
            try
            {
                var order = await _orderService.CompleteOrderAsync(id);
                return Ok(order);
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {id} не найден");
            }
            catch (InvalidOrderStateException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}