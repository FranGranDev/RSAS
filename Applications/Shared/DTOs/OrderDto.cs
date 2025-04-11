using Application.Models;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "ID склада обязателен")]
        public int? StockId { get; set; }

        [Required(ErrorMessage = "Имя клиента обязательно")]
        [StringLength(100, ErrorMessage = "Имя клиента не должно превышать 100 символов")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Контактный телефон обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Тип оплаты обязателен")]
        public Order.PaymentTypes PaymentType { get; set; }

        [Required(ErrorMessage = "Список товаров обязателен")]
        [MinLength(1, ErrorMessage = "Заказ должен содержать хотя бы один товар")]
        public IEnumerable<CreateOrderProductDto> Products { get; set; }

        [Required(ErrorMessage = "Информация о доставке обязательна")]
        public CreateDeliveryDto Delivery { get; set; }
    }

    public class UpdateOrderDto
    {
        [Required(ErrorMessage = "ID склада обязателен")]
        public int? StockId { get; set; }

        [Required(ErrorMessage = "Имя клиента обязательно")]
        [StringLength(100, ErrorMessage = "Имя клиента не должно превышать 100 символов")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Контактный телефон обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Тип оплаты обязателен")]
        public Order.PaymentTypes PaymentType { get; set; }

        [Required(ErrorMessage = "Статус заказа обязателен")]
        public Order.States State { get; set; }

        [Required(ErrorMessage = "Список товаров обязателен")]
        [MinLength(1, ErrorMessage = "Заказ должен содержать хотя бы один товар")]
        public IEnumerable<UpdateOrderProductDto> Products { get; set; }

        [Required(ErrorMessage = "Информация о доставке обязательна")]
        public UpdateDeliveryDto Delivery { get; set; }
    }
}