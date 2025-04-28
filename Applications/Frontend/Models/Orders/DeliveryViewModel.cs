using System.ComponentModel.DataAnnotations;
using Application.DTOs;

namespace Frontend.Models.Orders
{
    public class DeliveryViewModel
    {
        public bool Editable { get; set; } = true;

        public CreateDeliveryDto Delivery { get; set; } = new CreateDeliveryDto();

        [Display(Name = "Город")]
        [Required(ErrorMessage = "Город обязателен для заполнения")]
        [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
        public string City
        {
            get => Delivery.City;
            set => Delivery.City = value;
        }

        [Display(Name = "Улица")]
        [Required(ErrorMessage = "Улица обязательна для заполнения")]
        [StringLength(100, ErrorMessage = "Название улицы не должно превышать 100 символов")]
        public string Street
        {
            get => Delivery.Street;
            set => Delivery.Street = value;
        }

        [Display(Name = "Дом")]
        [Required(ErrorMessage = "Номер дома обязателен для заполнения")]
        [StringLength(10, ErrorMessage = "Номер дома не должен превышать 10 символов")]
        public string House
        {
            get => Delivery.House;
            set => Delivery.House = value;
        }

        [Display(Name = "Квартира")]
        [StringLength(10, ErrorMessage = "Номер квартиры не должен превышать 10 символов")]
        public string Flat
        {
            get => Delivery.Flat;
            set => Delivery.Flat = value;
        }

        [Display(Name = "Почтовый индекс")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Почтовый индекс должен состоять из 6 цифр")]
        public string PostalCode
        {
            get => Delivery.PostalCode;
            set => Delivery.PostalCode = value;
        }

        [Display(Name = "Дата доставки")]
        [Required(ErrorMessage = "Дата доставки обязательна для заполнения")]
        public DateTime DeliveryDate
        {
            get => Delivery.DeliveryDate;
            set => Delivery.DeliveryDate = value;
        }

        public int OrderId { get; set; }

        public CreateDeliveryDto ToDto() => Delivery;
    }
} 