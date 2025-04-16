namespace Frontend.Models.Catalog;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public int TotalQuantity => Items.Sum(x => x.Quantity);
    public decimal TotalPrice => Items.Sum(x => x.TotalPrice);
} 