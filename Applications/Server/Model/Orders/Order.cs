﻿using Application.Model.Stocks;
using Application.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using Application.Model.Sales;

namespace Application.Model.Orders
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? StockId { get; set; }

        public string ClientName { get; set; }
        public string ContactPhone { get; set; }

        public PaymentTypes PaymentType { get; set; }

        public DateTime ChangeDate { get; set; }
        public DateTime OrderDate { get; set; }
        public SaleTypes Type { get; set; }
        public States State { get; set; }



        public virtual AppUser User { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual Stock Stock { get; set; }
        public virtual ICollection<OrderProduct> Products { get; set; }

        public enum States
        {
            [Display(Name = "В обработке")]
            New,
            [Display(Name = "В работе")]
            InProcess,
            [Display(Name = "Отложено")]
            OnHold,
            [Display(Name = "Отменено")]
            Cancelled,
            [Display(Name = "Готово")]
            Completed,
        }
        public enum PaymentTypes
        {
            [Display(Name = "Наличными")]
            Cash,
            [Display(Name = "Картой")]
            Card,
            [Display(Name = "Банковский перевод")]
            Bank,
        }
    }
}
