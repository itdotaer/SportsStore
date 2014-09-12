using System.Collections.Generic;
using System.Linq;

namespace SportsStore.Domain.Entities
{
    //The core code of cart.
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public void AddProduct(Product product, int quantity)
        {
            var line = lineCollection.FirstOrDefault(l => l.Product.ProductID == product.ProductID);

            if (line == null)
            {
                lineCollection.Add(new CartLine{Product = product, Quantity = quantity});
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product)
        {
            lineCollection.RemoveAll(l => l.Product.ProductID == product.ProductID);
        }

        public decimal CoputeTotalValue()
        {
            return lineCollection.Sum(l => l.Quantity*l.Product.Price);
        }

        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
        }
    }

    //Struct the shopping cart data.
    public class CartLine
    {
        public Product Product { set; get; }
        public int Quantity { set; get; }
    }
}
