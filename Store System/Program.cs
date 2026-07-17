using System;
using Store_System.Models;
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

        string? option;

        do
        {
            DisplayOptions();

            option = Console.ReadLine();

            if (int.TryParse(option, out int intOption))
            {
                if (intOption >= 1 && intOption <= 12)
                {
                    //all the logic will be called here
                }

                else if (intOption == 13)
                {
                    Console.WriteLine("Good Bye");
                    break;
                }    

                else
                {
                    Console.WriteLine("INVALID OPTION: MUST BE BETWEEN 1 AND 13");
                }
            }

            else
            {
                Console.WriteLine("INVALID OPTION: MUST BE NUMBER");
            }

        } while (true);
    }
}