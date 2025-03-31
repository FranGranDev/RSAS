using Application.Model.Stocks;
using System.ComponentModel.DataAnnotations;

namespace Application.Model.Orders
{
    public class OrderProduct
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        
        public int Quantity { get; set; }

        //duplicate info, for analytics.
        public decimal ProductPrice { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
    }
}
