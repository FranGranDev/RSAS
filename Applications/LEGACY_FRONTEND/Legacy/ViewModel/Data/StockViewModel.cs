using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Application.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.ViewModel.Data
{
    public class StockViewModel : InputViewModel
    {
        public StockViewModel()
        {
        }

        public StockViewModel(Stock stock)
        {
            Id = stock.Id;
            Name = stock.Name;
            SaleType = stock.SaleType;
            Location = stock.Location;
        }

        public int Id { get; set; }

        [DisplayName("Название")] [Required] public string Name { get; set; }

        [DisplayName("Адрес")] [Required] public string Location { get; set; }

        [DisplayName("Тип склада")] [Required] public Stock.Types SaleType { get; set; }

        public IEnumerable<SelectListItem> GetEnumList()
        {
            return Enum.GetValues(typeof(Stock.Types))
                .Cast<Stock.Types>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.GetType()
                        .GetMember(e.ToString())[0]
                        .GetCustomAttributes(typeof(DisplayAttribute), false)
                        .FirstOrDefault() is DisplayAttribute attribute
                        ? attribute.Name
                        : e.ToString()
                });
        }
    }

    public class QuantityStockViewModel : StockViewModel
    {
        public QuantityStockViewModel()
        {
        }

        public QuantityStockViewModel(Stock stock) : base(stock)
        {
            if (stock.StockProducts == null)
            {
                Quantity = 0;
            }
            else
            {
                Quantity = stock.StockProducts.Select(x => x.Quantity).Sum();
            }
        }

        public int Quantity { get; set; }
    }
}