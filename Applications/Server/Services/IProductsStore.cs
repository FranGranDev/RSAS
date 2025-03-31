using Application.Model.Stocks;

namespace Application.Services
{
    public interface IProductsStore
    {
        public IQueryable<Product> All { get; }
        public Product Get(int id);
        public void Save(Product product);
        public void Delete(Product product);
    }
}
