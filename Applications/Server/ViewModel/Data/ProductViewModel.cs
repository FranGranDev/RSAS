using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Application.Model.Stocks;
using Application.Services;
using System.Configuration;

namespace Application.ViewModel.Data
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {

        }
        public ProductViewModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            RetailPrice = product.RetailPrice;
            WholesalePrice = product.WholesalePrice;
            Description = product.Description;
            Barcode = product.Barcode;
            Category = product.Category;
        }

        public int Id { get; set; }


        [DisplayName("Название")]
        [Required]
        public string Name { get; set; }


        [DisplayName($"Оптовая цена {CurrencySettings.Symbol}")]
        [DataType(DataType.Currency)]
        [Required, Range(0, double.MaxValue)]
        public decimal WholesalePrice { get; set; }


        [DisplayName($"Розничная цена {CurrencySettings.Symbol}")]
        [DataType(DataType.Currency)]
        [Required, Range(0, double.MaxValue)]
        public decimal RetailPrice { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Штрих-код")]
        [Required]
        [StringLength(50)]
        public string Barcode { get; set; }

        [DisplayName("Категория")]
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
    }
    public class QuantityProductViewModel: ProductViewModel
    {
        public QuantityProductViewModel()
        {

        }
        public QuantityProductViewModel(Product product) : base(product)
        {
            Quantity = product.StockProducts
                .Select(x => x.Quantity)
                .Sum();
        }

        [DisplayName("Количество")]
        [IntegerValidator(MinValue = 0)]
        public int Quantity { get; set; }
    }
}
