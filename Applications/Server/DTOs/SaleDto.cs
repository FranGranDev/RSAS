using System;

namespace Application.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int StockId { get; set; }
        public string StockName { get; set; }
        public DateTime SaleDate { get; set; }
    }

    public class CreateSaleDto
    {
        public int OrderId { get; set; }
        public int StockId { get; set; }
    }

    public class UpdateSaleDto
    {
        public int OrderId { get; set; }
        public int StockId { get; set; }
    }
} 