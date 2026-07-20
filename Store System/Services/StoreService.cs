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
        public void AddCustomer(Customer customer) => Customers.Add(customer);
        public void AddProduct(Product product) => Products.Add(product);
        public void CreateOrder()
        {
            Console.WriteLine(@$"==========Create Order Form==========");

            Console.WriteLine("insert your Id: ");

            string CustomerId = Console.ReadLine();

            //should i throw an error, or "invalid Id" msg to the user?
            if (string.IsNullOrWhiteSpace(CustomerId) || !Customers.Any(c => c.Id == CustomerId))
            {
                Console.WriteLine("invalid Id, order canceled");
                return;
            }

            Order newOrder = new Order(CustomerId);

            Orders.Add(newOrder);

            //this is the best logic I imaged to handle the items and quantities insertion, and it can re-enter the same item multiple times 
            Console.WriteLine("Enter the items in this form 'item1, quantity1' and press ENTER then 'item2, quantity' and press 0 to exit ");

            Dictionary<string, int> DictionaryItems = new Dictionary<string, int>();// DictionaryItems <item, qunatity>

            //the process of traslate the text into dictionary <item, quantity>
            while (true)
            {
                Console.WriteLine("enter items and quantity:");

                string UserInput = Console.ReadLine();

                if (UserInput == "0") { break; }// for exit the loop

                string[] saperatedInput = UserInput.Split(',');

                saperatedInput.Select(input =>
                {
                    input.Trim();
                    input.ToLower();
                    return input;
                });

                string item = saperatedInput[0];

                string quantity = saperatedInput[1];

                if (!int.TryParse(quantity, out int intQuantity))
                {
                    Console.WriteLine("invalid quantity, re-enter the item, quantity:");
                    continue;
                }

                DictionaryItems[item] = intQuantity;
                // final shape of data, rawItemsData<string item, int quantity>
            }

            //merge the duplicate items
            foreach (var itemQuantity in DictionaryItems)
            {
                //TryAdd: it try to add the key if its not added, return true if its new, and false if its already included
                if (!DictionaryItems.TryAdd(itemQuantity.Key, itemQuantity.Value))
                {
                    DictionaryItems[itemQuantity.Key] += itemQuantity.Value;
                }
            }


            //i think there is better way to iterate through the dictionary and products (join)
            foreach (var itemQuantity in DictionaryItems)
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


            Customer CurrentCustomer = Customers.FirstOrDefault(c => c.Id == CustomerId);

            //not need to nullity check, i already check it in the begin of the method CreateOrder
            decimal finalTotal = newOrder.CalculateFinalTotal(CurrentCustomer);


            //there is a very better solution but i stuck here for a long time, i will comeback to it later
            foreach (var item in newOrder.Items)
            {
                foreach (var product in Products)
                {
                    if (item.ProductId == product.Id)
                    {
                        product.ReduceStock(item.Quantity);
                    }
                }
            }

            /*
             var innerJoin = employees.Join(
                    departments,
                    emp => emp.DepartmentId,
                    dept => dept.Id,
                    (emp, dept) => new { emp.Name, DeptName = dept.Name }
                );
            */

            newOrder.CompleteOrder();

            DisplayOrderInfo(CurrentCustomer, newOrder);//i moved it to a method to make the CreateOrder cleaner
        }


        public void CancelOrder(string orderId)
        {
            Order? currentOrder = Orders.FirstOrDefault(order => order.Id == orderId);

            if (currentOrder == null || currentOrder.Status == OrderStatus.Cancelled)
            {
                //just to save some time
                Console.WriteLine("the order isn't exist or order already cancelled");
                return;
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
