using Application.DTOs;

namespace Frontend.Models.Stocks;

public class StockProductCardViewModel
{
    public StockProductDto StockProduct { get; set; }
    public ProductDto Product { get; set; }
    public string AddAction { get; set; }
    public string RemoveAction { get; set; }
    public string UpdateAction { get; set; }
    public string ManageUrl { get; set; }
} 