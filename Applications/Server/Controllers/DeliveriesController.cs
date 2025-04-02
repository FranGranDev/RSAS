using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    /// <summary>
    ///     Контроллер для управления доставками
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveriesController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        /// <summary>
        ///     Получить список всех доставок
        /// </summary>
        /// <returns>Список доставок</returns>
        /// <response code="403">Недостаточно прав для просмотра всех доставок</response>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveries()
        {
            var deliveries = await _deliveryService.GetAllDeliveriesAsync();
            return Ok(deliveries);
        }

        /// <summary>
        ///     Получить доставку по ID
        /// </summary>
        /// <param name="id">ID доставки</param>
        /// <returns>Информация о доставке</returns>
        /// <response code="403">Недостаточно прав для просмотра доставки</response>
        /// <response code="404">Доставка не найдена</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<DeliveryDto>> GetDelivery(int id)
        {
            try
            {
                var delivery = await _deliveryService.GetDeliveryByIdAsync(id);
                return Ok(delivery);
            }
            catch (DeliveryNotFoundException)
            {
                return NotFound($"Доставка с ID {id} не найдена");
            }
        }

        /// <summary>
        ///     Получить доставку по ID заказа
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Информация о доставке</returns>
        /// <response code="403">Недостаточно прав для просмотра доставки</response>
        /// <response code="404">Доставка не найдена</response>
        [HttpGet("order/{orderId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<DeliveryDto>> GetDeliveryByOrderId(int orderId)
        {
            try
            {
                var delivery = await _deliveryService.GetDeliveryByOrderIdAsync(orderId);
                return Ok(delivery);
            }
            catch (DeliveryNotFoundException)
            {
                return NotFound($"Доставка для заказа с ID {orderId} не найдена");
            }
        }

        /// <summary>
        ///     Создать новую доставку
        /// </summary>
        /// <param name="createDeliveryDto">Данные для создания доставки</param>
        /// <returns>Созданная доставка</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Заказ не найден</response>
        [HttpPost]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<DeliveryDto>> CreateDelivery(CreateDeliveryDto createDeliveryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delivery = await _deliveryService.CreateDeliveryAsync(createDeliveryDto);
                return CreatedAtAction(nameof(GetDelivery), new { id = delivery.Id }, delivery);
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {createDeliveryDto.OrderId} не найден");
            }
        }

        /// <summary>
        ///     Обновить информацию о доставке
        /// </summary>
        /// <param name="id">ID доставки</param>
        /// <param name="updateDeliveryDto">Данные для обновления доставки</param>
        /// <returns>Обновленная доставка</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="403">Недостаточно прав для обновления доставки</response>
        /// <response code="404">Доставка не найдена</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<DeliveryDto>> UpdateDelivery(int id, UpdateDeliveryDto updateDeliveryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delivery = await _deliveryService.UpdateDeliveryAsync(id, updateDeliveryDto);
                return Ok(delivery);
            }
            catch (DeliveryNotFoundException)
            {
                return NotFound($"Доставка с ID {id} не найдена");
            }
        }

        /// <summary>
        ///     Удалить доставку
        /// </summary>
        /// <param name="id">ID доставки</param>
        /// <returns>Результат операции</returns>
        /// <response code="403">Недостаточно прав для удаления доставки</response>
        /// <response code="404">Доставка не найдена</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            try
            {
                await _deliveryService.DeleteDeliveryAsync(id);
                return NoContent();
            }
            catch (DeliveryNotFoundException)
            {
                return NotFound($"Доставка с ID {id} не найдена");
            }
        }

        /// <summary>
        ///     Получить доставки по статусу
        /// </summary>
        /// <param name="status">Статус доставки</param>
        /// <returns>Список доставок</returns>
        /// <response code="403">Недостаточно прав для просмотра доставок</response>
        [HttpGet("status/{status}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveriesByStatus(string status)
        {
            var deliveries = await _deliveryService.GetDeliveriesByStatusAsync(status);
            return Ok(deliveries);
        }

        /// <summary>
        ///     Получить доставки по диапазону дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <returns>Список доставок</returns>
        /// <response code="403">Недостаточно прав для просмотра доставок</response>
        [HttpGet("date-range")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveriesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var deliveries = await _deliveryService.GetDeliveriesByDateRangeAsync(startDate, endDate);
            return Ok(deliveries);
        }
    }
}