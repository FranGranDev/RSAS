using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class DeliveryDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Flat { get; set; }
        public string PostalCode { get; set; }
    }

    public class CreateDeliveryDto
    {
        [Required(ErrorMessage = "Дата доставки обязательна")]
        [DataType(DataType.DateTime)]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Город обязателен")]
        [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
        public string City { get; set; }

        [Required(ErrorMessage = "Улица обязательна")]
        [StringLength(100, ErrorMessage = "Название улицы не должно превышать 100 символов")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Номер дома обязателен")]
        [StringLength(10, ErrorMessage = "Номер дома не должен превышать 10 символов")]
        public string House { get; set; }

        [Required(ErrorMessage = "Номер квартиры обязателен")]
        [StringLength(10, ErrorMessage = "Номер квартиры не должен превышать 10 символов")]
        public string Flat { get; set; }

        [Required(ErrorMessage = "Почтовый индекс обязателен")]
        [StringLength(10, ErrorMessage = "Почтовый индекс не должен превышать 10 символов")]
        public string PostalCode { get; set; }
    }

    public class UpdateDeliveryDto
    {
        [Required(ErrorMessage = "Дата доставки обязательна")]
        [DataType(DataType.DateTime)]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Город обязателен")]
        [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
        public string City { get; set; }

        [Required(ErrorMessage = "Улица обязательна")]
        [StringLength(100, ErrorMessage = "Название улицы не должно превышать 100 символов")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Номер дома обязателен")]
        [StringLength(10, ErrorMessage = "Номер дома не должен превышать 10 символов")]
        public string House { get; set; }

        [Required(ErrorMessage = "Номер квартиры обязателен")]
        [StringLength(10, ErrorMessage = "Номер квартиры не должен превышать 10 символов")]
        public string Flat { get; set; }

        [Required(ErrorMessage = "Почтовый индекс обязателен")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Почтовый индекс должен содержать 6 цифр")]
        public string PostalCode { get; set; }
    }
}