using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class Product
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }

        public virtual ICollection<StockProducts> StockProducts { get; set; }
    }
}