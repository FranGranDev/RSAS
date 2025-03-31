using Application.Model.Orders;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Model.Stocks
{
    public class Stock
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public Types SaleType { get; set; }


        public virtual ICollection<StockProducts> StockProducts { get; set; }

        public enum Types
        {
            [Display(Name = "Розничный")]
            Retail,
            [Display(Name = "Оптовый")]
            Wholesale,
        }
    }
}
