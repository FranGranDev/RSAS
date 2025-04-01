using System;
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
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Город обязателен")]
        public string City { get; set; }

        [Required(ErrorMessage = "Улица обязательна")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Номер дома обязателен")]
        public string House { get; set; }

        [Required(ErrorMessage = "Номер квартиры обязателен")]
        public string Flat { get; set; }

        [Required(ErrorMessage = "Почтовый индекс обязателен")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Почтовый индекс должен содержать 6 цифр")]
        public string PostalCode { get; set; }
    }

    public class UpdateDeliveryDto
    {
        [Required(ErrorMessage = "Дата доставки обязательна")]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Город обязателен")]
        public string City { get; set; }

        [Required(ErrorMessage = "Улица обязательна")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Номер дома обязателен")]
        public string House { get; set; }

        [Required(ErrorMessage = "Номер квартиры обязателен")]
        public string Flat { get; set; }

        [Required(ErrorMessage = "Почтовый индекс обязателен")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Почтовый индекс должен содержать 6 цифр")]
        public string PostalCode { get; set; }
    }
} 