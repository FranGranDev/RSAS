using Application.DTOs;
using Application.Model.Orders;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
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

        [HttpGet]
        public ActionResult<IEnumerable<DeliveryDto>> GetDeliveries()
        {
            var deliveries = _deliveryService.GetAllDeliveries()
                .Select(d => new DeliveryDto
                {
                    Id = d.OrderId,
                    OrderId = d.OrderId,
                    DeliveryDate = d.DeliveryDate,
                    City = d.City,
                    Street = d.Street,
                    House = d.House,
                    Flat = d.Flat,
                    PostalCode = d.PostalCode
                })
                .ToList();

            return Ok(deliveries);
        }

        [HttpGet("{id}")]
        public ActionResult<DeliveryDto> GetDelivery(int id)
        {
            var delivery = _deliveryService.GetDelivery(id);
            if (delivery == null)
            {
                return NotFound();
            }

            var deliveryDto = new DeliveryDto
            {
                Id = delivery.OrderId,
                OrderId = delivery.OrderId,
                DeliveryDate = delivery.DeliveryDate,
                City = delivery.City,
                Street = delivery.Street,
                House = delivery.House,
                Flat = delivery.Flat,
                PostalCode = delivery.PostalCode
            };

            return Ok(deliveryDto);
        }

        [HttpPost]
        public ActionResult<DeliveryDto> CreateDelivery(CreateDeliveryDto createDeliveryDto)
        {
            var delivery = _deliveryService.CreateDelivery(createDeliveryDto.OrderId, createDeliveryDto.DeliveryDate, createDeliveryDto.City, createDeliveryDto.Street, createDeliveryDto.House, createDeliveryDto.Flat, createDeliveryDto.PostalCode);
            if (delivery == null)
            {
                return NotFound("Заказ не найден");
            }

            return CreatedAtAction(nameof(GetDelivery), new { id = delivery.OrderId }, delivery);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDelivery(int id, UpdateDeliveryDto updateDeliveryDto)
        {
            var delivery = _deliveryService.GetDelivery(id);
            if (delivery == null)
            {
                return NotFound();
            }

            if (delivery.Order.State == Order.States.Cancelled)
            {
                return BadRequest("Нельзя изменить доставку отмененного заказа");
            }

            delivery.DeliveryDate = updateDeliveryDto.DeliveryDate;
            delivery.City = updateDeliveryDto.City;
            delivery.Street = updateDeliveryDto.Street;
            delivery.House = updateDeliveryDto.House;
            delivery.Flat = updateDeliveryDto.Flat;
            delivery.PostalCode = updateDeliveryDto.PostalCode;

            _deliveryService.UpdateDelivery(delivery);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDelivery(int id)
        {
            var delivery = _deliveryService.GetDelivery(id);
            if (delivery == null)
            {
                return NotFound();
            }

            if (delivery.Order.State == Order.States.Completed)
            {
                return BadRequest("Нельзя удалить доставку завершенного заказа");
            }

            _deliveryService.DeleteDelivery(delivery);
            return NoContent();
        }
    }
}