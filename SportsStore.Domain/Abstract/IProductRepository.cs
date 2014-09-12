using System.Linq;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        void CreateProduct(Product product);
        void SaveProduct(Product product);
        Product DeleteProduct(int productId);
    }
}
