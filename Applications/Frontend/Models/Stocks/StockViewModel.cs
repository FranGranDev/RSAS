using Application.DTOs;

namespace Frontend.Models.Stocks;

public class StockViewModel
{
    public StockDto Stock { get; set; }
    public IEnumerable<StockProductCardViewModel> Products { get; set; }
} 