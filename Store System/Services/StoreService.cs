using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Services
{
    internal class StoreService
    {
        private readonly List<Customer> Customers = [];
        private readonly List<Product> Products = [];
        private readonly List<Order> Orders = [];

        //should i keep it as it is, or pass the ID validate the customer/product here in these methods?

        public void DisplayOrderInfo(Customer CurrentCustomer, Order newOrder)
        {
            var ProductsItemsJoin = newOrder.Items.Join(
                Products,
                item => item.ProductId,
                prod => prod.Id,
                (item, prod) => new { productName = prod.Name, itemQuantity = item.Quantity, price = prod.Price }
                );

            Console.WriteLine($@"
==========Order Summary==========
Customer Name: {CurrentCustomer.FullName}
Customer Type: {CurrentCustomer.CustomerType}

Items:
{string.Join(Environment.NewLine, ProductsItemsJoin.Select(nameQuantity =>
$"{nameQuantity.productName} x {nameQuantity.itemQuantity} = {nameQuantity.itemQuantity * nameQuantity.price}"))}

Subtotal: {newOrder.CalculateSubtotal()}
Discount: {CurrentCustomer.GetDiscountPercentage() * newOrder.CalculateSubtotal()}
Final Total: {newOrder.CalculateFinalTotal(CurrentCustomer)}
");
        }
        public void AddCustomer(string name, string email, CustomerType accountType)
        {
            //add try-catch block
            Customer newCustomer = new Customer(name, email, accountType);

            Customers.Add(newCustomer);
        }


        public void AddProduct(string name, string category, decimal price, int quantity)
        {
            //add try-catch block
            Product newProduct = new Product(name, category, price, quantity);
            Products.Add(newProduct);
        }


        public void CreateOrder(string customerId, Dictionary<string, int> itemQuantityDictionary)
        {
            //test purposes
            //foreach (var item in itemQuantityDictionary)
            //{
            //    Console.WriteLine($"test (the beginning of CreateOrder): {item.Key}, {item.Value}");
            //}

            Customer? CurrentCustomer = Customers.FirstOrDefault(c => c.Id == customerId);

            if (CurrentCustomer == null)
            {
                //throw new exception
                throw new Exception("adsfasf");
            }

            //add try-catch block
            Order newOrder = new Order(customerId);

            Orders.Add(newOrder);


            //merge the duplicate items (there is a problem here the dictionary itself doesnot allow duplicates so the problem is the actual handling of data) 
            Dictionary<string, int> container = new Dictionary<string, int>();
            foreach (var itemQuantity in itemQuantityDictionary)
            {
                //TryAdd: it try to add the key if its not added, return true if its new, and false if its already included
                if (!container.TryAdd(itemQuantity.Key, itemQuantity.Value))
                {
                    container[itemQuantity.Key] += itemQuantity.Value;
                }
            }
            itemQuantityDictionary = container;

            //i think there is better way to iterate through the dictionary and products (join)
            foreach (var itemQuantity in itemQuantityDictionary)
            {
                foreach (var product in Products)
                {
                    if (product.Name == itemQuantity.Key && product.QuantityInStock >= itemQuantity.Value)
                    {
                        OrderItem newItem = new OrderItem(product.Id, itemQuantity.Value, product.Price);

                        newOrder.AddItem(newItem);
                    }
                }
            }
            
            decimal finalTotal = newOrder.CalculateFinalTotal(CurrentCustomer);

            var matchedOrderItems = newOrder.Items.Join(
                Products,
                item => item.ProductId,
                product => product.Id,
                (item, product) => new { Item = item, Product = product }
                );

            foreach (var match in matchedOrderItems)
            {
                match.Product.ReduceStock(match.Item.Quantity);
            }

            //Console.WriteLine($"test: quantity{newOrder.Items.FirstOrDefault().Quantity}, itemId:{newOrder.Items[0].ProductId}");

            newOrder.CompleteOrder();

            DisplayOrderInfo(CurrentCustomer, newOrder);
            //i moved it to a function to make the CreateOrder cleaner
        }


        public void CancelOrder(string orderId)
        {
            Order? currentOrder = Orders.FirstOrDefault(order => order.Id == orderId);

            if (currentOrder == null || currentOrder.Status == OrderStatus.Cancelled)
            {
                //throw a new exception
                //i should saperate the two conditions
                throw new Exception("no order with that Id/ order status cancelled");
            }


            //here i handeled the case of empty Order (Pending order)
            if (!currentOrder.Items.Any())
            {
                currentOrder.CancelOrder();
                return;
            }

            //mapping the product with items in order to return the quantity to product stock
            var matchedProducts = currentOrder.Items.Join(
                Products,
                item => item.ProductId,
                product => product.Id,
                (item, product) => new
                {
                    targetProduct = product,
                    AmountToIncrease = item.Quantity
                });

            foreach (var match in matchedProducts)
            {
                match.targetProduct.IncreaseStock(match.AmountToIncrease);
            }

            currentOrder.CancelOrder();
        }

        public void DefferedAndImmediateExecution()
        {
            var products = Products.ToList();//ToList to be able to change the list

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


        public IQueryable<Product> IEnumerableAndIQueryable()
        {
            IQueryable<Product> queryableProducts = Products.AsQueryable();

            return queryableProducts.Where(product => product.Price >= 100);
        }
        /*
        * limitations:
        * 
        * even though we are using IQueryable here, the underlying data source 
        * is still an in memory List because of this:
        * 
        * 1. no sql translation: calling AsQueryable() on a List does not magically 
        *    connect the application to a database The execution still happens in RAM
        *    
        * 2. translation limits: if this were a true database connection (like EF), 
        *    we would be severely limited in what we could put inside the .Where() clause 
        *    a database provider cannot translate custom c# methods into raw sql, which 
        *    would cause runtime translation exceptions
        */


        public IEnumerable<Customer> GetCustomers() => Customers.AsEnumerable();
        public IEnumerable<Product> GetProducts() => Products.AsEnumerable();
        public IEnumerable<Order> GetOrders() => Orders.AsEnumerable();


    }
}
