using System;
using Store_System.Models;
using Store_System.Services;
class Program
{

    public static void DisplayOptions()
    {
        Console.WriteLine(@"
========== SALES AND ORDER ANALYSIS SYSTEM ==========
1. Add Customer
2. Add Product
3. Create Order
4. Cancel Order
5. Display Customers
6. Display Products
7. Display Orders
8. Search Products
9. Display Sales Reports
10. Demonstrate Deferred Execution
11. Demonstrate IEnumerable and IQueryable
12. Demonstrate Iterators
13. Exit
Select an option:
            ");
    }

    static void Main()
    {
        StoreService service = new StoreService();

        string? option;

        while (true)
        {
            DisplayOptions();

            option = Console.ReadLine();

            if (int.TryParse(option, out int intOption))
            {
                switch (intOption)
                {
                    case 1:
                        //add new customer
                        Console.WriteLine("Enter Your Name: ");

                        string? inputCustomerName = Console.ReadLine();

                        //i already validate all the user input in the class constructors, by throwing a new error
                        if (string.IsNullOrWhiteSpace(inputCustomerName))
                        {
                            Console.WriteLine("invalid name: is_null_or_whitespaces");
                            continue;
                        }

                        Console.WriteLine("Enter your Email: ");

                        string? inputEmail = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputEmail))
                        {
                            Console.WriteLine("invalid email: is_null_or_whitespaces");
                            continue;
                        }

                        Console.WriteLine("Choise your account type: p->premium, r->regular");

                        string? inputAccountType = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputAccountType))
                        {
                            Console.WriteLine("invalid accout type: is_null_or_whitespaces");
                            continue;
                        }

                        CustomerType type;

                        if (inputAccountType == "p")
                        {
                            type = CustomerType.Premium;
                        }

                        else if (inputAccountType == "r")
                        {
                            type = CustomerType.Regular;
                        }

                        else
                        {
                            Console.WriteLine("invalid account type: must be either 'p' or 'r'");
                            continue;
                        }

                        service.AddCustomer(inputCustomerName, inputEmail, type);

                        break;

                    case 2:
                        //add new product
                        Console.WriteLine("enter the product name: ");

                        string? inputProductName = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputProductName))
                        {
                            Console.WriteLine("invalid name: is_null_or_whitespaces");
                            continue;
                        }

                        Console.WriteLine("enter the product category: ");

                        string? inputProductCategory = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputProductCategory))
                        {
                            Console.WriteLine("invalid category: is_null_or_whitespaces");
                            continue;
                        }

                        Console.WriteLine("enter the product price: ");

                        string? inputProductPrice = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputProductPrice) || !decimal.TryParse(inputProductPrice, out decimal intProductPrice) || intProductPrice <= 0)
                        {
                            Console.WriteLine("invalid price: should be not null and a number and more than zero ");
                            continue;
                        }

                        Console.WriteLine("enter the product quantity: ");

                        string? inputProductQuantity = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputProductQuantity) || !int.TryParse(inputProductQuantity, out int intProductQuantity) || intProductQuantity < 0)
                        {

                            //i should use saperatezd conditions to show the user there is his problem
                            Console.WriteLine("invalid price: should be not null and a number and more than or equal to zero");
                            continue;
                        }

                        service.AddProduct(inputProductName, inputProductCategory, intProductPrice, intProductQuantity);
                        break;

                    case 3:
                        //create order

                        Console.WriteLine(@$"==========Create Order Form==========");

                        Console.WriteLine("enter customer Id: ");

                        string? inputCustomerId = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputCustomerId))
                        {
                            Console.WriteLine("invalid Id: is_null_or_whitespaces ");
                            continue;
                        }

                        //this is the best logic I imaged to handle the items and quantities insertion, and it can re-enter the same item multiple times 
                        Console.WriteLine("Enter the items in this form 'item1, quantity1' and press ENTER then 'item2, quantity' and press 0 to exit ");

                        Dictionary<string, int> DictionaryItems = new Dictionary<string, int>();// DictionaryItems <item, qunatity>

                        //the process of traslate the text into dictionary <item, quantity>
                        while (true)
                        {
                            Console.WriteLine("enter items and quantity:");

                            string? inputItemQuantity = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(inputItemQuantity))
                            {
                                Console.WriteLine("invalid item: is_null_or_whitespace");
                                continue;
                            }

                            if (inputItemQuantity.Trim() == "0") { break; }// for exit the loop

                            string[] saperatedInput = inputItemQuantity.Split(',');

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

                        service.CreateOrder(inputCustomerId, DictionaryItems);

                        break;

                    case 4:
                        //cancel order
                        Console.WriteLine("enter the order id: ");

                        string? inputOrderId = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inputOrderId))
                        {
                            Console.WriteLine("invalid order id: is_null_or_whitespace");
                            continue;
                        }

                        service.CancelOrder(inputOrderId);
                        break;

                    case 5:
                        //dispaly customers
                        IEnumerable<Customer> customers = service.GetCustomers();

                        foreach (var customer in customers)
                        {
                            customer.DisplayInfo();
                        }

                        break;

                    case 6:
                        //display products
                        IEnumerable<Product> products = service.GetProducts();

                        foreach (var product in products)
                        {
                            product.DisplayInfo();
                        }

                        break;

                    case 7:
                        //display orders
                        IEnumerable<Order> orders = service.GetOrders();

                        foreach (var order in orders)
                        {
                            //order.DisplayInfo();
                        }
                        break;
                    
                    case 8:
                        //search Products

                        break;

                    case 9:
                        //display slaes reports
                        
                        break;

                    case 10:
                        //demonstrate deferred execution

                        break;

                    case 11:
                        //demonstrate IEnumerable and IQuaryable

                        break;

                    case 12:
                        //demonstrate Iterators

                        break;


                    case 13:
                        Console.WriteLine("Good Bye");
                        return;

                    default:
                        Console.WriteLine("INVALID OPTION: MUST BE BETWEEN 1 AND 13");
                        break;
                           
                }

            }

            else
            {
                Console.WriteLine("INVALID OPTION: MUST BE NUMBER");
            }

        }
    }
}