using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public string ClientName { get; set; }

        [Required]
        public string ClientPhone { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string? Comment { get; set; }

        public ICollection<SaleProductDto> Products { get; set; } = new List<SaleProductDto>();
    }

    public class SaleProductDto
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductCategory { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        public decimal DiscountAmount { get; set; }
    }
} 