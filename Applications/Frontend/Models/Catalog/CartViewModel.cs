namespace Frontend.Models.Catalog;

public class CartViewModel
{
    public int StockId { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new();
    public int TotalQuantity => Items.Sum(x => x.Quantity);
    public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
}