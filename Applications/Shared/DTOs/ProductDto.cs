using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Название товара обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Название товара не должно превышать 100 символов")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Цена товара обязательна для заполнения")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена товара должна быть больше 0")]
        public decimal Price { get; set; }
        [StringLength(500, ErrorMessage = "Описание товара не должно превышать 500 символов")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Штрих-код товара обязателен для заполнения")]
        [StringLength(50, ErrorMessage = "Штрих-код товара не должен превышать 50 символов")]
        public string Barcode { get; set; }
        [Required(ErrorMessage = "Категория товара обязательна для заполнения")]
        [StringLength(100, ErrorMessage = "Категория товара не должна превышать 100 символов")]
        public string Category { get; set; }
    }
}