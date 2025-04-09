using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Application.Attributes;

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