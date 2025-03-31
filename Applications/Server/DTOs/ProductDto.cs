using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public string Description { get; set; }
        public ICollection<StockProductDto> StockProducts { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public string Description { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public string Description { get; set; }
    }
} 