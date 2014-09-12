using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EFProductRepository:IProductRepository
    {
        private readonly EFDbContext _dbContext = new EFDbContext();

        public IQueryable<Product> Products { get { return _dbContext.Products; }}
        public void CreateProduct(Product product)
        {
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                _dbContext.Products.Add(product);
            }
            else
            {
                var dbEntry = _dbContext.Products.Find(product.ProductID);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                }
            }
            _dbContext.SaveChanges();
        }

        public Product DeleteProduct(int productId)
        {
            var findProd = _dbContext.Products.FirstOrDefault(prod => prod.ProductID == productId);

            if (findProd != null)
            {
                _dbContext.Products.Remove(findProd);
                _dbContext.SaveChanges();
            }

            return findProd;
        }
    }
}
