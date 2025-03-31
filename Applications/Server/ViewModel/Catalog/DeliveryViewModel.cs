using Application.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Catalog
{
    public class DeliveryViewModel : InputViewModel
    {
        [Required(ErrorMessage = "Укажите дату доставки")]
        
        [DisplayName("Дата доставки")]
        [DataType(DataType.DateTime)]
        [ValidateDateRange]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Укажите {0}")]
        [DisplayName("Город")]
        public string City { get; set; }
        [Required(ErrorMessage = "Укажите Улицу")]
        [DisplayName("Улица")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Укажите {0}")]
        [DisplayName("Номер дома")]
        public string House { get; set; }
        [Required(ErrorMessage = "Укажите {0}")]
        [DisplayName("Номер квартиры")]
        public string Flat { get; set; }
        [Required(ErrorMessage = "Укажите {0}")]
        [DisplayName("Почтовый индекс")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
    }
}
