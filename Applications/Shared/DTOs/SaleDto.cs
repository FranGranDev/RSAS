using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ID заказа обязателен")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Номер заказа обязателен")]
        [StringLength(50, ErrorMessage = "Номер заказа не должен превышать 50 символов")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "ID склада обязателен")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Название склада обязательно")]
        [StringLength(100, ErrorMessage = "Название склада не должно превышать 100 символов")]
        public string StockName { get; set; }

        [Required(ErrorMessage = "Дата продажи обязательна")]
        [DataType(DataType.DateTime)]
        public DateTime SaleDate { get; set; }

        [Required(ErrorMessage = "Общая сумма обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Общая сумма должна быть больше 0")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Имя клиента обязательно")]
        [StringLength(100, ErrorMessage = "Имя клиента не должно превышать 100 символов")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Телефон клиента обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string ClientPhone { get; set; }

        [Required(ErrorMessage = "Список товаров обязателен")]
        [MinLength(1, ErrorMessage = "Продажа должна содержать хотя бы один товар")]
        public ICollection<SaleProductDto> Products { get; set; } = new List<SaleProductDto>();
    }
    
    public class SaleProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ID продажи обязателен")]
        public int SaleId { get; set; }

        [Required(ErrorMessage = "ID товара обязателен")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(100, ErrorMessage = "Название товара не должно превышать 100 символов")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Категория товара обязательна")]
        [StringLength(100, ErrorMessage = "Категория товара не должна превышать 100 символов")]
        public string ProductCategory { get; set; }

        [Required(ErrorMessage = "Количество товара обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество товара должно быть больше 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Цена товара обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена товара должна быть больше 0")]
        public decimal ProductPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Сумма скидки должна быть неотрицательной")]
        public decimal DiscountAmount { get; set; }
    }
}