using System.ComponentModel.DataAnnotations;

namespace Application.Model.Stocks
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public virtual ICollection<StockProducts> StockProducts { get; set; }
    }
}
