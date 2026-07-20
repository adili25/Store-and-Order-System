using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class DeferredAndImmediateExecution
    {
        public void DefferedAndImmediateExecution(IEnumerable<Product> _products)
        {
            var products = _products.ToList();// this outside the code, just to be able to add to the list


            var availableQuery = products.Where(product => product.QuantityInStock > 0);

            products.Add(new Product("Laptop", "Electronics", 900m, 5));
            // we add the products before the execution so its have been added

            foreach (var product in availableQuery)//here the query "availableQuary" got executed
            {
                Console.WriteLine(product.Name);
            }
            //-------------------

            var availableList = products.Where(product => product.QuantityInStock > 0).ToList();
            // here is the query "availableList" got executed

            products.Add(new Product("Computer", "Electronics", 1200m, 4));
            //we add the products after the execution so it will not be shown, we must execute it again after adding the product

            foreach (var product in availableList)
            {
                Console.WriteLine(product.Name);
            }

            //
            /* 
             * Defferred execution: in the defferred exection (delayed execution) the instuction it self is ready but not excuted yet, i can continue modify it and when i finish modifying i can call the execution methods such as: ToList(), ToArray(), or using foreach (streaming execution)
             
             * Immediate execution: this execute the method directly without any waiting
              
             * 1- the danger of Immediate:
                    every time ToList() executed, a tone of RAM is used to create a new List with all the values
              
             * 2- the power of Defferred:
                   if you didn't use ToList() the query almost takes zero RAM memory
                   it just set the rules and not execute it untill you call ToList(), ToArray() or foreach(..)
             *   
             * 
             */
        }
    }
}
