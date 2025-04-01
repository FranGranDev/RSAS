using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class StockDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ICollection<StockProductDto> StockProducts { get; set; }
    }

    public class CreateStockDto
    {
        [Required(ErrorMessage = "Название склада обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Название склада не должно превышать 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Адрес склада обязателен для заполнения")]
        [StringLength(200, ErrorMessage = "Адрес склада не должно превышать 200 символов")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Город обязателен для заполнения")]
        [StringLength(100, ErrorMessage = "Город не должен превышать 100 символов")]
        public string City { get; set; }

        [Required(ErrorMessage = "Телефон склада обязателен для заполнения")]
        [StringLength(20, ErrorMessage = "Телефон склада не должен превышать 20 символов")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email склада обязателен для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
        public string Email { get; set; }
    }

    public class UpdateStockDto
    {
        [Required(ErrorMessage = "Название склада обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Название склада не должно превышать 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Адрес склада обязателен для заполнения")]
        [StringLength(200, ErrorMessage = "Адрес склада не должно превышать 200 символов")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Город обязателен для заполнения")]
        [StringLength(100, ErrorMessage = "Город не должен превышать 100 символов")]
        public string City { get; set; }

        [Required(ErrorMessage = "Телефон склада обязателен для заполнения")]
        [StringLength(20, ErrorMessage = "Телефон склада не должен превышать 20 символов")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email склада обязателен для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
        public string Email { get; set; }
    }
}