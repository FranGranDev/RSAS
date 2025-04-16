namespace Frontend.Models.Catalog;

public class CatalogViewModel
{
    public List<CatalogProductViewModel> Products { get; set; }
    public List<string> Categories { get; set; }
    public CartViewModel Cart { get; set; }
} 