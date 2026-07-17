using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Services
{
    internal class StoreService
    {
        private readonly List<Customer> customers;
        private readonly List<Product> products;
        private readonly List<Order> orders;
        public void AddCustomer(Customer customer) => customers.Add(customer);
        public void AddProduct(Product product) => products.Add(product);
        public void CreateOrder()
        {
            Console.WriteLine(@$"==========Create Order Form==========");
            
            string CustomerId = Console.ReadLine();

            //here the null check before the contains check, should it be correct?
            if (!(string.IsNullOrEmpty(CustomerId) && customers.Any(c => c.Id == CustomerId)))
            {
                Console.WriteLine("invalid Id, order canceled");
                return;
            }

            //this is the best logic i found to handle the the items and quantities, and it can re-enter the same item multiple times 
            Console.WriteLine("Enter the items in this form 'item1, quantity' and press ENTER then 'item2, quantity' and press 0 to exit ");
            
            Dictionary<string, int> rawItemsData = new Dictionary<string, int>();// rawItemsData <item, qunatity>
            
            //the process of traslate the text into dictionary <item, quantity>
            while (true)
            {
                Console.WriteLine("enter items and quantity:");

                string inputItems = Console.ReadLine();

                if (inputItems == "0") { break; }
                
                string[] speratedItemAndQuantity = inputItems.Split(',');

                speratedItemAndQuantity.Select(input => input.Trim());

                string quantity = speratedItemAndQuantity[1];

                string item = speratedItemAndQuantity[0];

                if (!int.TryParse(quantity, out int intQuantity))
                {
                    Console.WriteLine("invalid quantity, orderd cancled");
                    return;
                }

                rawItemsData[item] = intQuantity;
                // final shape of data, rawItemsData <string item, int quantity>
            }

            List<OrderItem> listOfOrderItems = new List<OrderItem>();

            foreach (KeyValuePair<string, int> pair in rawItemsData)
            {
                listOfOrderItems.Add(new OrderItem());
            }

            Order newOrder = new Order()


        }
        public void CancelOrder(int orderId)
        public IEnumerable<Customer> GetCustomers()
        public IEnumerable<Product> GetProducts()
        public IEnumerable<Order> GetOrders()
    }
}
