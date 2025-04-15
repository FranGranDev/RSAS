using Application.DTOs;

namespace Frontend.Models.Stocks;

public class StockProductCardViewModel
{
    public StockProductDto StockProduct { get; set; }
    public ProductDto Product { get; set; }
    public int StockId { get; set; }
    public string ManageUrl { get; set; }
} 