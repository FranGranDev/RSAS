﻿using Application.Model.Orders;
using Application.Model.Sales;
using Application.ViewModel.Catalog;
using System.ComponentModel;
using static Application.Model.Orders.Order;

namespace Application.ViewModel.Orders
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            
        }
        public OrderViewModel(Order order)
        {
            Id = order.Id;
            ClientName = order.ClientName;
            ContactPhone = order.ContactPhone;
            ChangeDate = order.ChangeDate;
            OrderDate = order.OrderDate;
            PaymentType = order.PaymentType;
            State = order.State;
            Type = order.Type;
            Amount = order.Products.Select(x => x.ProductPrice * x.Quantity).Sum();
            Description = string.Join(" | ", order.Products.Select(p => $"Товар {p.ProductName}, Количество {p.Quantity}"));
            Quantity = order.Products.Select(x => x.Quantity).Sum();
        }

        public int Id { get; set; }

        [DisplayName("ФИО")]
        public string ClientName { get; set; }
        [DisplayName("Телефон")]
        public string ContactPhone { get; set; }

        [DisplayName("Дата изменения состояния")]
        public DateTime ChangeDate { get; set; }
        [DisplayName("Дата заказа")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Тип заказа")]
        public SaleTypes Type { get; set; }
        [DisplayName("Состояние заказа")]
        public States State { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Сумма заказа")]
        public decimal Amount { get; set; }
        [DisplayName("Количество товаров")]
        public int Quantity { get; set; }
        [DisplayName("Тип оплаты")]
        public PaymentTypes PaymentType { get; set; }


        public string StateBgColor
        {
            get
            {
                switch (State)
                {
                    case States.New:
                        return "#FFFACD";
                    case States.Cancelled:
                        return "#FFB6C1";
                    case States.InProcess:
                        return "#90EE90";
                    case States.OnHold:
                        return "#FFE4C4";
                    case States.Completed:
                        return "#20B2AA";
                    default:
                        return "#90EE90";
                }
            }
        }
        public string TextColor
        {
            get
            {
                switch (State)
                {
                    default:
                        return "#343a40";
                    case States.InProcess:
                        return "#ffffff";
                    case States.Completed:
                        return "#ffffff";
                }
            }
        }
    }
}
