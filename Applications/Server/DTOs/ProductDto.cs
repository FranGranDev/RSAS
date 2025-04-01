using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public ICollection<StockProductDto> StockProducts { get; set; }
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "Название товара обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Название товара не должно превышать 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Цена товара обязательна для заполнения")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена товара должна быть больше 0")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "Описание товара не должно превышать 500 символов")]
        public string Description { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Название товара обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Название товара не должно превышать 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Цена товара обязательна для заполнения")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена товара должна быть больше 0")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "Описание товара не должно превышать 500 символов")]
        public string Description { get; set; }
    }
} 