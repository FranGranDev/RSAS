using Application.Model.Orders;
using Application.Model.Sales;
using Application.ViewModel.Catalog;
using Application.ViewModel.Data;
using Application.ViewModel.Orders;
using Application.ViewModel.Users;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Sales
{
    public class SaleViewModel
    {
        public SaleViewModel(Sale sale, CompanyViewModel company = null)
        {
            Id = sale.Id;
            UserId = sale.Order.UserId;

            StockView = new StockViewModel(sale.Stock);
            OrderView = new OrderViewModel(sale.Order);

            SaleType = sale.SaleType;
            SaleDate = sale.SaleDate;
            OrderDate = sale.Order.OrderDate;

            ClientInfo = new ContactInfoViewModel()
            {
                FullName = OrderView.ClientName,
                Phone = OrderView.ContactPhone,
                Disabled = true,
            };
            CompanyInfo = company;

            Delivery delivery = sale.Order.Delivery;
            DeliveryView = new DeliveryViewModel()
            {
                DeliveryDate = delivery.DeliveryDate,
                House = delivery.House,
                City = delivery.City,
                Flat = delivery.Flat,
                Street = delivery.Street,
                PostalCode = delivery.PostalCode,
                Disabled = true,
            };

            Products = sale.Order.Products;
            Amount = Products.Select(x => x.ProductPrice * x.Quantity).Sum();
            Quantity = Products.Select(x => x.Quantity).Sum();
        }

        public int Id { get; }
        public string UserId { get; }


        public StockViewModel StockView { get; }
        public OrderViewModel OrderView { get; }
        public DeliveryViewModel DeliveryView { get; set; }
        public ContactInfoViewModel ClientInfo { get; set; }
        public CompanyViewModel CompanyInfo { get; set; }


        [Display(Name = "Тип продажи")]
        public SaleTypes SaleType { get; }

        [Display(Name = "Дата заявки")]
        public DateTime OrderDate { get; }

        [Display(Name = "Дата продажи")]
        public DateTime SaleDate { get; }

        [Display(Name = "Выручка")]
        public decimal Amount { get; }
        [Display(Name = "Количество товаров")]
        public int Quantity { get; }

        public IEnumerable<OrderProduct> Products { get; }
    }
}
