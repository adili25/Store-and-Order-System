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

                        string? inputName = Console.ReadLine();

                        //i already validate all the user input in the class constructors, by throwing a new error
                        if (string.IsNullOrWhiteSpace(inputName))
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

                        service.AddCustomer(inputName, inputEmail, inputAccountType);
                        break;

                    case 2:
                        //add new product

                        break;

                    case 3:
                        //crete order

                        break;

                    case 4:
                        //cancel order

                        break;

                    case 5:
                        //dispaly customers

                        break;

                    case 6:
                        //display products

                        break;

                    case 7:
                        //display orders
                        
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