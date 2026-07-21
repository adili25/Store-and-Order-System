using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Services
{
    internal class StoreService
    {
        private readonly List<Customer> Customers = new List<Customer>();
        private readonly List<Product> Products = new List<Product>();
        private readonly List<Order> Orders = new List<Order>();

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
Discount: {newOrder.CalculateFinalTotal(CurrentCustomer) - newOrder.CalculateSubtotal()}
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
            if (!Customers.Any(c => c.Id == customerId))
            {
                //throw new exception
            }

            //add try-catch block
            Order newOrder = new Order(customerId);

            Orders.Add(newOrder);

            //merge the duplicate items
            foreach (var itemQuantity in itemQuantityDictionary)
            {
                //TryAdd: it try to add the key if its not added, return true if its new, and false if its already included
                if (!itemQuantityDictionary.TryAdd(itemQuantity.Key, itemQuantity.Value))
                {
                    itemQuantityDictionary[itemQuantity.Key] += itemQuantity.Value;
                }
            }

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

            //what is the customer Id was not found? => in the begging of the method i check if customer exits or not
            Customer CurrentCustomer = Customers.FirstOrDefault(c => c.Id == customerId);
            
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


        public IEnumerable<Customer> GetCustomers() => Customers.AsEnumerable();
        public IEnumerable<Product> GetProducts() => Products.AsEnumerable();
        public IEnumerable<Order> GetOrders() => Orders.AsEnumerable();


    }
}
