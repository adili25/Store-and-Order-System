using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Iterators
{
    internal class ProductIterator
    {
        public static IEnumerable<Product> GetLowStockProducts(IEnumerable<Product> products, int stockLimit)
        {
            if (stockLimit <= 0)
            {
                yield break;
            }

            foreach (var product in products)
            {
                if (product.QuantityInStock >= 0 && product.QuantityInStock < stockLimit)
                {
                    yield return product;
                }
            }
        }

        public static IEnumerable<Product> GetTopExpensiveProducts(IEnumerable<Product> products, int maximumResults)
        {
            if (maximumResults <= 0)
            {
                yield break;
            }

            products.OrderByDescending(p => p.Price);

            int returnCounter = 0;

            foreach (var product in products)
            {
                if (returnCounter > maximumResults)
                {
                    yield break;
                }

                yield return product;

                returnCounter++;
            }
        }
    }
}
