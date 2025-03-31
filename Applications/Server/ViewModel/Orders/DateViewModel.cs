using Application.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Application.ViewModel.Orders
{
    public class DateViewModel
    {
        [DisplayName("Дата доставки")]
        [DataType(DataType.DateTime)]
        [ValidateDateRange]
        public DateTime Date { get; set; }
    }
}
