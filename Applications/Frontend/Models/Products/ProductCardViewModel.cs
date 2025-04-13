using Application.DTOs;

namespace Frontend.Models.Products;

public class ProductCardViewModel
{
    public ProductDto Product { get; set; }
    public string EditAction { get; set; }
} 