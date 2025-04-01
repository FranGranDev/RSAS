using System;
using Application.Model.Sales;

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
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; }
        public string StatusDisplay { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
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
        public SaleStatus Status { get; set; }
    }
} 