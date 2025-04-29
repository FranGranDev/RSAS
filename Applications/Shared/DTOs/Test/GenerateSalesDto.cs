using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Test
{
    public class GenerateSalesDto
    {
        [Required(ErrorMessage = "Дата начала периода обязательна")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Дата окончания периода обязательна")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Количество продаж обязательно")]
        [Range(1, 1000, ErrorMessage = "Количество продаж должно быть от 1 до 1000")]
        public int SalesCount { get; set; }

        [Required(ErrorMessage = "Средняя продолжительность заказа обязательна")]
        [Range(1, 100000, ErrorMessage = "Средняя продолжительность заказа должна быть от 1 до 100000 минут")]
        public int AverageOrderDurationMinutes { get; set; }

        [Required(ErrorMessage = "Минимальное количество товаров в заказе обязательно")]
        [Range(1, 10, ErrorMessage = "Минимальное количество товаров должно быть от 1 до 10")]
        public int MinProductsPerOrder { get; set; } = 1;

        [Required(ErrorMessage = "Максимальное количество товаров в заказе обязательно")]
        [Range(1, 20, ErrorMessage = "Максимальное количество товаров должно быть от 1 до 10")]
        public int MaxProductsPerOrder { get; set; } = 5;

        [Required(ErrorMessage = "Минимальное количество каждого товара обязательно")]
        [Range(1, 100, ErrorMessage = "Минимальное количество товара должно быть от 1 до 100")]
        public int MinProductQuantity { get; set; } = 1;

        [Required(ErrorMessage = "Максимальное количество каждого товара обязательно")]
        [Range(1, 100, ErrorMessage = "Максимальное количество товара должно быть от 1 до 100")]
        public int MaxProductQuantity { get; set; } = 10;

        [Required(ErrorMessage = "Минимальное количество дней до доставки обязательно")]
        [Range(1, 30, ErrorMessage = "Минимальное количество дней до доставки должно быть от 1 до 30")]
        public int MinDeliveryDays { get; set; } = 1;

        [Required(ErrorMessage = "Максимальное количество дней до доставки обязательно")]
        [Range(1, 30, ErrorMessage = "Максимальное количество дней до доставки должно быть от 1 до 30")]
        public int MaxDeliveryDays { get; set; } = 4;
    }
} 