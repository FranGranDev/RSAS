using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class OrderProductDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreateOrderProductDto
    {
        [Required(ErrorMessage = "ID товара обязателен")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(100, ErrorMessage = "Название товара не должно превышать 100 символов")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Описание товара обязательно")]
        [StringLength(500, ErrorMessage = "Описание товара не должно превышать 500 символов")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Цена товара обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Количество товара обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }
    }

    public class UpdateOrderProductDto
    {
        [Required(ErrorMessage = "ID товара обязателен")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Цена товара обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Количество товара обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }
    }
}