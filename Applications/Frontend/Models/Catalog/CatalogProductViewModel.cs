namespace Frontend.Models.Catalog;

public class CatalogProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Barcode { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int MaxQuantity { get; set; }
} 