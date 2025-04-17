using System.ComponentModel.DataAnnotations;
using Application.Models;

namespace Frontend.Models.Orders
{
    public class PaymentViewModel
    {
        public bool Editable { get; set; } = true;

        [Display(Name = "Тип оплаты")]
        [Required(ErrorMessage = "Тип оплаты обязателен для выбора")]
        public Order.PaymentTypes PaymentType { get; set; }

        [Display(Name = "Сумма к оплате")]
        public decimal Amount { get; set; }
    }
} 