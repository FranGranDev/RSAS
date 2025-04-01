using Application.Models;
using Application.ViewModel.Data;

namespace Application.ViewModel.Catalog
{
    public class CatalogItemViewModel : QuantityProductViewModel
    {
        public CatalogItemViewModel(SaleTypes saleType)
        {
            PriceType = saleType;
        }

        public CatalogItemViewModel(SaleTypes saleType, Product product) : base(product)
        {
            PriceType = saleType;
        }


        public int TakenCount { get; set; }

        public decimal ProductPrice
        {
            get
            {
                if (PriceType == SaleTypes.Retail)
                {
                    return RetailPrice;
                }

                return WholesalePrice;
            }
        }

        public decimal TotalPrice { get; set; }
        public SaleTypes PriceType { get; set; }
    }
}