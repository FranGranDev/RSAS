using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Model.Orders;
using Application.Services;
using Application.Model.Stocks;
using Application.Exceptions;
using Application.Services.Repository;

namespace Application.Controllers
{
    /// <summary>
    /// Контроллер для управления заказами
    /// </summary>
    [Authorize(Policy = "RequireManagerRole")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderStore _orderStore;
        private readonly IStockStore _stockStore;
        private readonly IStockProductsStore _stockProductsStore;
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderStore orderStore,
            IStockStore stockStore,
            IStockProductsStore stockProductsStore,
            IOrderService orderService)
        {
            _orderStore = orderStore;
            _stockStore = stockStore;
            _stockProductsStore = stockProductsStore;
            _orderService = orderService;
        }

        /// <summary>
        /// Получить список всех заказов
        /// </summary>
        /// <returns>Список заказов</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Информация о заказе</returns>
        /// <response code="404">Заказ не найден</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Заказ с ID {id} не найден", ex);
            }
        }

        /// <summary>
        /// Создать новый заказ
        /// </summary>
        /// <param name="createOrderDto">Данные для создания заказа</param>
        /// <returns>Созданный заказ</returns>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные заказа", ModelState);
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException("Клиент или товар не найден", ex);
            }
        }

        /// <summary>
        /// Обновить существующий заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <param name="updateOrderDto">Данные для обновления заказа</param>
        /// <returns>Обновленный заказ</returns>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Заказ не найден</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, UpdateOrderDto updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException("Некорректные данные заказа", ModelState);
            }

            try
            {
                var order = await _orderService.UpdateOrderAsync(id, updateOrderDto);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Заказ с ID {id} не найден", ex);
            }
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Результат операции</returns>
        /// <response code="404">Заказ не найден</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Заказ с ID {id} не найден", ex);
            }
        }

        /// <summary>
        /// Получить заказы клиента
        /// </summary>
        /// <param name="clientId">ID клиента</param>
        /// <returns>Список заказов клиента</returns>
        /// <response code="404">Клиент не найден</response>
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetClientOrders(int clientId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByClientIdAsync(clientId);
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                throw new BusinessException($"Клиент с ID {clientId} не найден", ex);
            var orders = _orderStore.All
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    UserName = o.User.UserName,
                    StockId = o.StockId,
                    StockName = o.Stock.Name,
                    ClientName = o.ClientName,
                    ContactPhone = o.ContactPhone,
                    PaymentType = o.PaymentType,
                    PaymentTypeDisplay = o.PaymentType.ToString(),
                    ChangeDate = o.ChangeDate,
                    OrderDate = o.OrderDate,
                    State = o.State,
                    StateDisplay = o.State.ToString(),
                    TotalAmount = o.Products.Sum(p => p.ProductPrice * p.Quantity),
                    Products = o.Products.Select(p => new OrderProductDto
                    {
                        Id = p.Id,
                        ProductId = p.ProductId,
                        ProductName = p.Product.Name,
                        Quantity = p.Quantity,
                        ProductPrice = p.ProductPrice
                    }),
                    Delivery = o.Delivery != null ? new DeliveryDto
                    {
                        Id = o.Delivery.Id,
                        DeliveryDate = o.Delivery.DeliveryDate,
                        City = o.Delivery.City,
                        Street = o.Delivery.Street,
                        House = o.Delivery.House,
                        Flat = o.Delivery.Flat,
                        PostalCode = o.Delivery.PostalCode
                    } : null
                })
                .ToList();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public ActionResult<OrderDto> GetOrder(int id)
        {
            var order = _orderStore.Get(id);
            if (order == null)
                throw new OrderNotFoundException(id);

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User.UserName,
                StockId = order.StockId,
                StockName = order.Stock.Name,
                ClientName = order.ClientName,
                ContactPhone = order.ContactPhone,
                PaymentType = order.PaymentType,
                PaymentTypeDisplay = order.PaymentType.ToString(),
                ChangeDate = order.ChangeDate,
                OrderDate = order.OrderDate,
                State = order.State,
                StateDisplay = order.State.ToString(),
                TotalAmount = order.Products.Sum(p => p.ProductPrice * p.Quantity),
                Products = order.Products.Select(p => new OrderProductDto
                {
                    Id = p.Id,
                    ProductId = p.ProductId,
                    ProductName = p.Product.Name,
                    Quantity = p.Quantity,
                    ProductPrice = p.ProductPrice
                }),
                Delivery = order.Delivery != null ? new DeliveryDto
                {
                    Id = order.Delivery.Id,
                    DeliveryDate = order.Delivery.DeliveryDate,
                    City = order.Delivery.City,
                    Street = order.Delivery.Street,
                    House = order.Delivery.House,
                    Flat = order.Delivery.Flat,
                    PostalCode = order.Delivery.PostalCode
                } : null
            };

            return Ok(orderDto);
        }

        [HttpPost]
        [Authorize(Policy = "RequireManagerRole")]
        public ActionResult<OrderDto> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createOrderDto.StockId.HasValue)
            {
                var stock = _stockStore.Get(createOrderDto.StockId.Value);
                if (stock == null)
                    throw new StockNotFoundException(createOrderDto.StockId.Value);
            }

            var order = new Order
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                StockId = createOrderDto.StockId,
                ClientName = createOrderDto.ClientName,
                ContactPhone = createOrderDto.ContactPhone,
                PaymentType = createOrderDto.PaymentType,
                OrderDate = DateTime.Now,
                ChangeDate = DateTime.Now,
                State = Order.States.New,
                Products = createOrderDto.Products.Select(p => new OrderProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    ProductPrice = p.ProductPrice
                }).ToList(),
                Delivery = createOrderDto.Delivery != null ? new Delivery
                {
                    DeliveryDate = createOrderDto.Delivery.DeliveryDate,
                    City = createOrderDto.Delivery.City,
                    Street = createOrderDto.Delivery.Street,
                    House = createOrderDto.Delivery.House,
                    Flat = createOrderDto.Delivery.Flat,
                    PostalCode = createOrderDto.Delivery.PostalCode
                } : null
            };

            _orderStore.Save(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public IActionResult UpdateOrder(int id, UpdateOrderDto updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _orderStore.Get(id);
            if (order == null)
                throw new OrderNotFoundException(id);

            if (order.State == Order.States.Cancelled)
                throw new InvalidOrderStateException("Нельзя изменить отмененный заказ");

            if (updateOrderDto.StockId.HasValue)
            {
                var stock = _stockStore.Get(updateOrderDto.StockId.Value);
                if (stock == null)
                    throw new StockNotFoundException(updateOrderDto.StockId.Value);
            }

            order.StockId = updateOrderDto.StockId;
            order.ClientName = updateOrderDto.ClientName;
            order.ContactPhone = updateOrderDto.ContactPhone;
            order.PaymentType = updateOrderDto.PaymentType;
            order.State = updateOrderDto.State;
            order.ChangeDate = DateTime.Now;

            // Обновляем товары
            order.Products.Clear();
            order.Products = updateOrderDto.Products.Select(p => new OrderProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ProductPrice = p.ProductPrice
            }).ToList();

            // Обновляем доставку
            if (updateOrderDto.Delivery != null)
            {
                if (order.Delivery == null)
                    order.Delivery = new Delivery();

                order.Delivery.DeliveryDate = updateOrderDto.Delivery.DeliveryDate;
                order.Delivery.City = updateOrderDto.Delivery.City;
                order.Delivery.Street = updateOrderDto.Delivery.Street;
                order.Delivery.House = updateOrderDto.Delivery.House;
                order.Delivery.Flat = updateOrderDto.Delivery.Flat;
                order.Delivery.PostalCode = updateOrderDto.Delivery.PostalCode;
            }

            _orderStore.Save(order);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _orderStore.Get(id);
            if (order == null)
                throw new OrderNotFoundException(id);

            if (order.State == Order.States.Completed)
                throw new InvalidOrderStateException("Нельзя удалить завершенный заказ");

            _orderStore.Delete(id);
            return NoContent();
        }
    }
} 