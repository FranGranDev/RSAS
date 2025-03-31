namespace Application.Services
{
    public class DataManager
    {
        public DataManager(IStockProductsStore stockProducts, IStockStore stocks, IProductsStore products, IOrderStore orders, ISalesStore sales)
        {
            StockProducts = stockProducts;
            Stocks = stocks;
            Products = products;
            Orders = orders;
            Sales = sales;
        }

        public IStockProductsStore StockProducts { get; set; }
        public IStockStore Stocks { get; set; }
        public IProductsStore Products { get; set; }
        public IOrderStore Orders { get; set; }
        public ISalesStore Sales { get; set; }
    }
}
