using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models
{
    public class StockProducts
    {
        [ForeignKey(nameof(StockId))]
        [InverseProperty(nameof(StockProducts))]
        public int StockId { get; set; }

        public virtual Stock Stock { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty(nameof(StockProducts))]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}