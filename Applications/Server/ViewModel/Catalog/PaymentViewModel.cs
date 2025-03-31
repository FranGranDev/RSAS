using System.ComponentModel.DataAnnotations;
using static Application.Model.Orders.Order;

namespace Application.ViewModel.Catalog
{
    public class PaymentViewModel : InputViewModel
    {
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        public PaymentTypes PaymentType { get; set; }
    }
}
