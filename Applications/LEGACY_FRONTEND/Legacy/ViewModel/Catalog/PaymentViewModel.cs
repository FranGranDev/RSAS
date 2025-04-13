using System.ComponentModel.DataAnnotations;
using Application.Models;

namespace Application.ViewModel.Catalog
{
    public class PaymentViewModel : InputViewModel
    {
        [DataType(DataType.Currency)] public decimal Amount { get; set; }

        public Order.PaymentTypes PaymentType { get; set; }
    }
}