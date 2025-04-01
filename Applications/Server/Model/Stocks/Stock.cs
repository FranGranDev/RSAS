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
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public virtual ICollection<StockProducts> StockProducts { get; set; }
    }
}
