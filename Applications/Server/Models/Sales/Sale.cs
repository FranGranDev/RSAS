using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Models;

namespace Server.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [Required]
        public string ClientName { get; set; }

        [Required]
        public string ClientPhone { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string? Comment { get; set; }

        public virtual ICollection<SaleProduct> Products { get; set; } = new List<SaleProduct>();
    }
} 