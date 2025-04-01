using System;
using System.Collections.Generic;
using Application.Model.Orders;

namespace Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? StockId { get; set; }
        public string StockName { get; set; }
        public string ClientName { get; set; }
        public string ContactPhone { get; set; }
        public Order.PaymentTypes PaymentType { get; set; }
        public string PaymentTypeDisplay { get; set; }
        public DateTime ChangeDate { get; set; }
        public DateTime OrderDate { get; set; }
        public Order.States State { get; set; }
        public string StateDisplay { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<OrderProductDto> Products { get; set; }
        public DeliveryDto Delivery { get; set; }
    }

    public class CreateOrderDto
    {
        public int? StockId { get; set; }
        public string ClientName { get; set; }
        public string ContactPhone { get; set; }
        public Order.PaymentTypes PaymentType { get; set; }
        public IEnumerable<CreateOrderProductDto> Products { get; set; }
        public CreateDeliveryDto Delivery { get; set; }
    }

    public class UpdateOrderDto
    {
        public int? StockId { get; set; }
        public string ClientName { get; set; }
        public string ContactPhone { get; set; }
        public Order.PaymentTypes PaymentType { get; set; }
        public Order.States State { get; set; }
        public IEnumerable<UpdateOrderProductDto> Products { get; set; }
        public UpdateDeliveryDto Delivery { get; set; }
    }
} 