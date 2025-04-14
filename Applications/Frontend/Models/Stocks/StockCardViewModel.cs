using Application.DTOs;

namespace Frontend.Models.Stocks;

public class StockCardViewModel
{
    public StockDto Stock { get; set; }
    public string ClickAction { get; set; }
} 