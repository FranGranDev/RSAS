using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Model.Orders;
using Application.Services;

namespace Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveriesController : ControllerBase
    {
        private readonly IOrderStore _orderStore;

        public DeliveriesController(IOrderStore orderStore)
        {
            _orderStore = orderStore;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DeliveryDto>> GetDeliveries()
        {
            var deliveries = _orderStore.All
                .Where(o => o.Delivery != null)
                .Select(o => new DeliveryDto
                {
                    Id = o.Delivery.OrderId,
                    OrderId = o.Id,
                    DeliveryDate = o.Delivery.DeliveryDate,
                    City = o.Delivery.City,
                    Street = o.Delivery.Street,
                    House = o.Delivery.House,
                    Flat = o.Delivery.Flat,
                    PostalCode = o.Delivery.PostalCode
                })
                .ToList();

            return Ok(deliveries);
        }

        [HttpGet("{id}")]
        public ActionResult<DeliveryDto> GetDelivery(int id)
        {
            var order = _orderStore.Get(id);
            if (order?.Delivery == null)
                return NotFound();

            var deliveryDto = new DeliveryDto
            {
                Id = order.Delivery.OrderId,
                OrderId = order.Id,
                DeliveryDate = order.Delivery.DeliveryDate,
                City = order.Delivery.City,
                Street = order.Delivery.Street,
                House = order.Delivery.House,
                Flat = order.Delivery.Flat,
                PostalCode = order.Delivery.PostalCode
            };

            return Ok(deliveryDto);
        }

        [HttpPost]
        public ActionResult<DeliveryDto> CreateDelivery(CreateDeliveryDto createDeliveryDto)
        {
            var order = _orderStore.Get(createDeliveryDto.OrderId);
            if (order == null)
                return NotFound("Заказ не найден");

            if (order.Delivery != null)
                return BadRequest("Доставка для этого заказа уже существует");

            order.Delivery = new Delivery
            {
                DeliveryDate = createDeliveryDto.DeliveryDate,
                City = createDeliveryDto.City,
                Street = createDeliveryDto.Street,
                House = createDeliveryDto.House,
                Flat = createDeliveryDto.Flat,
                PostalCode = createDeliveryDto.PostalCode
            };

            _orderStore.Save(order);
            return CreatedAtAction(nameof(GetDelivery), new { id = order.Id }, order.Delivery);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDelivery(int id, UpdateDeliveryDto updateDeliveryDto)
        {
            var order = _orderStore.Get(id);
            if (order?.Delivery == null)
                return NotFound();

            if (order.State == Order.States.Cancelled)
                return BadRequest("Нельзя изменить доставку отмененного заказа");

            order.Delivery.DeliveryDate = updateDeliveryDto.DeliveryDate;
            order.Delivery.City = updateDeliveryDto.City;
            order.Delivery.Street = updateDeliveryDto.Street;
            order.Delivery.House = updateDeliveryDto.House;
            order.Delivery.Flat = updateDeliveryDto.Flat;
            order.Delivery.PostalCode = updateDeliveryDto.PostalCode;

            _orderStore.Save(order);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDelivery(int id)
        {
            var order = _orderStore.Get(id);
            if (order?.Delivery == null)
                return NotFound();

            if (order.State == Order.States.Completed)
                return BadRequest("Нельзя удалить доставку завершенного заказа");

            order.Delivery = null;
            _orderStore.Save(order);
            return NoContent();
        }
    }
} 