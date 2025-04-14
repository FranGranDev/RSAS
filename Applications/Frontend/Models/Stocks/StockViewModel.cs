using Application.DTOs;

namespace Frontend.Models.Stocks;

public class StockViewModel
{
    public StockDto Stock { get; set; }
    public IEnumerable<StockProductCardViewModel> Products { get; set; }
    public string SearchQuery { get; set; }
    public string SortBy { get; set; }
    public string SortDirection { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
} 