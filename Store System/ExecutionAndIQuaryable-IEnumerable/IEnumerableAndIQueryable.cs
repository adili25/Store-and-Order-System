using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal interface IEnumerableAndIQueryable
    {
        public void IEnumerableAndIQueryable(IEnumerable<Product> _products)
        {
            IQueryable<Product> queryableProducts = _products.AsQueryable();

            var query = queryableProducts.Where(product => product.Price > 100);
        }
     /*
     * REQUIRED EXPLANATION OF LIMITATIONS:
     * 
     * Even though we are using IQueryable<T> here, the underlying data source 
     * is still an in-memory List<T>. Because of this:
     * 
     * 1. No SQL Translation: Calling AsQueryable() on a List does not magically 
     *    connect the application to a database. The execution still happens in RAM.
     * 2. Translation Limits: If this were a true database connection (like Entity Framework), 
     *    we would be severely limited in what we could put inside the .Where() clause. 
     *    A database provider cannot translate custom C# methods into raw SQL, which 
     *    would cause runtime translation exceptions.
     */
        
    }
}
