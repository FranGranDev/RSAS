using Application.Model.Orders;
using Application.Model.Stocks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Model.Sales
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int StockId { get; set; }
        public DateTime SaleDate { get; set; }

        public virtual Order Order { get; set; }
        public virtual Stock Stock { get; set; }
    }
}
