using Store_System.Iterators;
using Store_System.Models;
using Store_System.Reports;
using Store_System.Services;
using System;
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
                            Console.WriteLine(item, intQuantity);
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
                            order.DisplayInfo();
                        }
                        break;
                    
                    case 8:
                        //search Products
                        Console.WriteLine("enter product name (or press enter for skip: ");

                        string? inputSearchName = Console.ReadLine();

                        Console.WriteLine("enter product category (or press enter for skip: ");

                        string? inputSearchCategory = Console.ReadLine();

                        Console.WriteLine("enter product min price (or press enter for skip: ");

                        string? inputSearchMinPrice = Console.ReadLine();

                        decimal.TryParse(inputSearchMinPrice, out decimal IntInputSearchMinPrice);

                        SalesReportService ProductSearchReport = new SalesReportService(service.GetProducts(), service.GetOrders(), service.GetCustomers());

                        var filteredProducts = ProductSearchReport.GetDynamicProductSearch(inputSearchName, inputSearchCategory, IntInputSearchMinPrice);
                        //here the query is ready we just iterate through it and get the products
                        if (filteredProducts == default)
                        {
                            Console.WriteLine("no Products to filter");
                        }

                        else
                        {
                            foreach (var product in filteredProducts)
                            {
                                product.DisplayInfo();
                            }
                        }
                        break;

                    case 9:
                        //display sales reports
                        var reportService = new SalesReportService(
                            service.GetProducts(),
                            service.GetOrders(),
                            service.GetCustomers()
                        );

                        //challenge 2: Product Projection
                        Console.WriteLine("[ Product Projection ]");
                        var ProductProjection = reportService.GetProductProjection();
                        if (!ProductProjection.Any())
                        {
                            Console.WriteLine("No Products in the system");
                        }
                        else
                        {
                            foreach (var p in reportService.GetProductProjection())
                            {
                                Console.WriteLine($"- {p.Name} ({p.Category}) | {p.Price} JOD | Status: {p.stockStatus}");
                            }
                        }

                        //challenge 3: Category Statistics
                        Console.WriteLine("[ Category Statistics ]");
                        var CagtegoryStatictics = reportService.CagtegoryStatistics();

                        if (!CagtegoryStatictics.Any())
                        {
                            Console.WriteLine("no Products in the system");
                        }
                        else
                        { 
                            foreach (var stat in CagtegoryStatictics)
                            {
                                Console.WriteLine($"- {stat.CategoryName}: {stat.ProductCount} products, {stat.TotalUnits} units available, Avg Price: {stat.AveragePrice:F2} JOD. (Most Exp: {stat.MostExpensive}, Least Exp: {stat.LeastExpensive})");
                            }
                        }

                        //challenge 4: Customer Order Report
                        Console.WriteLine("[ Customer Order Report ]");
                        var CustomerOrderRreport = reportService.GetCustomerOrderReport();
                        if (!CustomerOrderRreport.Any())
                        {
                            Console.WriteLine("No Orders/Customers in the system");
                        }
                        else
                        {
                            foreach (var report in CustomerOrderRreport)
                            {
                                Console.WriteLine($"- Order {report.OrderId} | {report.CustomerName} ({report.CustomerType}) | {report.OrderDate:yyyy-MM-dd} | Status: {report.OrderStatus} | Total: {report.OrderTotal} JOD");
                            }
                        }

                        //challenge 5: Order Detail Report
                        Console.WriteLine("[ Order Detail Report ]");
                        var OrderDetailReport = reportService.GetOrderDetailReport();
                        if (!OrderDetailReport.Any())
                        {
                            Console.WriteLine("No Orders/Products in the System");
                        }
                        else {
                            foreach (var detail in OrderDetailReport)
                            {
                                Console.WriteLine($"- Order {detail.OrderId} (Customer {detail.CustomerId}) | {detail.ProductName} x{detail.Quantity} | Unit: {detail.UnitPrice} JOD | Total: {detail.TotalPrice} JOD");
                            }
                        }

                        // challenge 6 & 7: Aggregates
                        Console.WriteLine("[ Aggregated Summaries ]");
                        decimal? TotalCompletedSales = reportService.CalculateTotalCompletedSales();
                        if (TotalCompletedSales != default)
                        {
                            Console.WriteLine($"Total Completed Sales Revenue: {TotalCompletedSales} JOD");
                        }
                        else
                        {
                            Console.WriteLine("No Completed Orders");
                        }
                        string? OrderItemSummary = reportService.OrderItemTextSummary();
                        if (OrderItemSummary != default)
                        {
                            Console.WriteLine($"Inventory Summary: {OrderItemSummary}");
                        }
                        else
                        {
                            Console.WriteLine("No Completed Orders");
                        }
                        // challenge 8: Customers Ranked by Spending
                        Console.WriteLine("[ CUSTOMERS RANKED BY SPENDING ]");
                        var rankedCustomers = reportService.CustomersRankedBySpending();
                        if (!rankedCustomers.Any())
                        {
                            Console.WriteLine("no Completed Orders");
                        }
                        else
                        {
                            int rank = 1;
                            foreach (var c in rankedCustomers)
                            {
                                Console.WriteLine($"{rank}. {c.CustomerName} | Orders: {c.CompletedOrdersCount} | Total: {c.TotalSpend:F2} JOD | Average: {c.AvgOrderValue:F2} JOD");
                                rank++;
                            }
                        }

                        //challenge 9: Best Selling Products
                        Console.WriteLine("\n[ Best Selling Products ]");
                        var bestSellers = reportService.BestSellingProducts();
                        if (!bestSellers.Any())
                        {
                            Console.WriteLine("No completed sales data available.");
                        }
                        else
                        {
                            foreach (var bs in bestSellers)
                            {
                                Console.WriteLine($"- {bs.ProductName} | Units Sold: {bs.QuantitySold} | Total Revenue: {bs.TotalSales:F2} JOD");
                            }
                        }

                        //challenge 10: Customers With No Orders
                        Console.WriteLine("\n[ Customers With No Orders ]");
                        var inactiveCustomers = reportService.CustomersWithNoOrders();
                        if (!inactiveCustomers.Any())
                        {
                            Console.WriteLine("All customers have placed at least one order.");
                        }
                        else
                        {
                            foreach (var customer in inactiveCustomers)
                            {
                                Console.WriteLine($"- {customer.FullName} ({customer.Email})");
                            }
                        }

                        //challenge 11: Most Valuable Order
                        Console.WriteLine("\n[ Most Valuable Order ]");
                        var topOrder = reportService.MostValuableOrder();
                        //default is the default value for what ever the type is (class => null, number=>0, bool=>false)
                        if (topOrder == default)
                        {
                            Console.WriteLine("no Orders/Customers in the system");
                        }
                        else
                        {
                            Console.WriteLine($"- Customer: {topOrder.CustomerName}");
                            Console.WriteLine($"- Subtotal: {topOrder.Subtotal:F2} JOD | Discount: {topOrder.Discount}% | Final Total: {topOrder.FinalTotal:F2} JOD");
                        }

                        // challenge 12: Monthly Sales Report
                        Console.WriteLine("\n[ MONTHLY SALES ]");
                        var monthlySales = reportService.GetMonthlySalesReport();
                        if (!monthlySales.Any())
                        {
                            Console.WriteLine("No completed sales data available.");
                        }
                        else
                        {
                            foreach (var month in monthlySales)
                            {
                                //formatting to match the requirements
                                Console.WriteLine($"{month.Year}-{month.Month:D2} | Orders: {month.OrderCount} | Total: {month.TotalSales:F2} JOD | Average: {month.AverageOrderValue:F2} JOD");
                            }
                        }

                        Console.WriteLine("\n================================================");

                        break;

                    case 10:
                        //demonstrate deferred execution
                        service.DefferedAndImmediateExecution();
                        Console.WriteLine("See The Implementation Inside The Method");
                        break;
                            
                    case 11:
                        //demonstrate IEnumerable and IQuaryable
                        var query = service.IEnumerableAndIQueryable();
                        if (query == null)
                        {
                            Console.WriteLine("No Orders With Price Above 100");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("=============Orders Above 100===========");
                            foreach (var product in query)
                            {
                                product.DisplayInfo();
                            }
                        }

                        break;

                    case 12:
                        //demonstrate Iterators

                        var allProducts = service.GetProducts();

                        var lowStockProducts = ProductIterator.GetLowStockProducts(allProducts, 5);

                        foreach (var product in lowStockProducts)
                        {
                            // this loop will fetch products one by one
                            Console.WriteLine($"- {product.Name} | Stock: {product.QuantityInStock}");
                        }


                        var topExpensiveProducts = ProductIterator.GetTopExpensiveProducts(allProducts, 3);

                        foreach (var product in topExpensiveProducts)
                        {
                            Console.WriteLine($"- {product.Name} | Price: {product.Price} JOD");
                        }

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