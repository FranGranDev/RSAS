using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class StockProductDto
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public string StockName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateStockProductDto
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

    public class UpdateStockProductDto
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