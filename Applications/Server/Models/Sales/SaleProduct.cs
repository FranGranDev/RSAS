using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Models;

namespace Server.Models
{
    public class SaleProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [ForeignKey(nameof(SaleId))]
        public Sale Sale { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductCategory { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
    }
} 