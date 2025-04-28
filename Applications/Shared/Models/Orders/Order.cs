using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class Order
    {
        public enum PaymentTypes
        {
            [Display(Name = "Наличными")] Cash,
            [Display(Name = "Картой")] Card,
            [Display(Name = "Банковский перевод")] Bank
        }

        public enum States
        {
            [Display(Name = "Новый")] New,
            [Display(Name = "В работе")] InProcess,
            [Display(Name = "Отложено")] OnHold,
            [Display(Name = "Отменено")] Cancelled,
            [Display(Name = "Готово")] Completed
        }

        [Key] public int Id { get; set; }

        public string UserId { get; set; }
        public int? StockId { get; set; }

        public string ClientName { get; set; }
        public string ContactPhone { get; set; }

        public PaymentTypes PaymentType { get; set; }

        public DateTime ChangeDate { get; set; }
        public DateTime OrderDate { get; set; }
        public States State { get; set; }
        
        public string CancellationReason { get; set; }

        public virtual AppUser User { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual Stock Stock { get; set; }
        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}