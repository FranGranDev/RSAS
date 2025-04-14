using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class StockProductDto
    {
        [Required(ErrorMessage = "ID склада обязателен")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "ID товара обязателен")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Количество товара обязательно")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество товара должно быть неотрицательным")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Цена товара обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена товара должна быть больше 0")]
        public decimal Price { get; set; }
    }
}