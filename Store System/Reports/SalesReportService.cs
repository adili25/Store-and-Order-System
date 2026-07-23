using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Store_System.Reports
{
    //here i will build a record for each report to make the class cleaner without these IEnumerable<(string ...)> return types
    public record ProductProjection(string Name, string Category, decimal Price, StockStatus stockStatus);
    public enum StockStatus{ OutOfStock, Available };
    public record MonthlySalesSummary(int Year, int Month, int OrderCount, decimal TotalSales, decimal AverageOrderValue);
    public record CategoryStatisticsSummary(string CategoryName, int ProductCount, int TotalUnits, decimal AveragePrice, string MostExpensive, string LeastExpensive);

    public record CustomerOrderReportSummary(string OrderId, string CustomerName, CustomerType CustomerType, DateTime OrderDate, OrderStatus OrderStatus, decimal OrderTotal);

    public record CustomerSpendingRanking(string CustomerName, int CompletedOrdersCount, decimal TotalSpend, decimal AvgOrderValue);

    public record BestSellingProductSummary(string ProductName, int QuantitySold, decimal TotalSales);

    public record MostValuableOrderSummary(string CustomerName, decimal Subtotal, decimal Discount, decimal FinalTotal);

    public record OrderDetailReportDto(string CustomerId, string OrderId, string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice);

    internal class SalesReportService
    {
        //these for save the list of products and the list of orders to handle all the LINQ
        private readonly IEnumerable<Product> _products;
        private readonly IEnumerable<Order> _orders;
        private readonly IEnumerable<Customer> _customers;


        public SalesReportService(IEnumerable<Product> products, IEnumerable<Order> orders, IEnumerable<Customer> customers)
        {
            _products = products;
            _orders = orders;
            _customers = customers;
        }


        //challenge 1 
        public IEnumerable<Product> GetDynamicProductSearch(string? name = null, string? category = null, decimal? minPrice = null)
        {
            IEnumerable<Product> filteredProducts = _products;

            if (!string.IsNullOrEmpty(name))
            {
                filteredProducts = filteredProducts.Where(p => p.Name == name);
            }

            if (!string.IsNullOrEmpty(category))
            {
                filteredProducts = filteredProducts.Where(p => p.Category == category);
            }

            if (!(minPrice == null))
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minPrice);
            }

            filteredProducts = filteredProducts.Where(p => p.QuantityInStock > 0);

            return filteredProducts;
        }
        
        //challenge 2
        public IEnumerable<ProductProjection> GetProductProjection()
        {
            //in this solution no need to build a whole new struct or class
            return _products.Select(p => new ProductProjection(
                p.Name,
                p.Category,
                p.Price,
                p.QuantityInStock > 0 ? StockStatus.Available : StockStatus.OutOfStock
            ));

            //if you wanna use this code change the IEnumerable type into ProductProjectionDto
            //IEnumerable<Product> query = _products;

            //IEnumerable<ProductProjectionDto> result = query.Select(p => 

            //    new ProductProjectionDto(
            //        p.Name,
            //        p.Category,
            //        p.Price,
            //        p.QuantityInStock > 0 ? StockStatus.Available : StockStatus.OutOfStock
            //        )
            //);

            //return result;
        }

        //challenge 3
        public IEnumerable<CategoryStatisticsSummary> CagtegoryStatistics()
        {
            //count(), Sum(), ....MinBy() nullable safe => the GroupBy gerenty that each group have at least one item 
            //what if the _products empty => the GroupBy safely return empty collection
            return _products.GroupBy(p => p.Category)
                            .Select(g => new CategoryStatisticsSummary(
                                CategoryName: g.Key,
                                ProductCount: g.Count(),
                                TotalUnits: g.Sum(g => g.QuantityInStock),
                                AveragePrice: g.Average(p => p.Price),
                                MostExpensive: g.MaxBy(p => p.Price)?.Name ?? "Unknown",
                                LeastExpensive: g.MinBy(p => p.Price)?.Name ?? "Unknown"
                                ));
        }
        

        //challenge 4
        public IEnumerable<CustomerOrderReportSummary> GetCustomerOrderReport()
        {
            return _orders.Join(
                _customers,
                o => o.CustomerId,
                c => c.Id,
                (order, customer) => new CustomerOrderReportSummary(
                    OrderId: order.Id,
                    CustomerName: customer.FullName,
                    CustomerType: customer.CustomerType,
                    OrderDate : order.OrderDate,
                    OrderStatus : order.Status,
                    OrderTotal : order.CalculateSubtotal()
                ));

        }


        //challenge 5
        public IEnumerable<OrderDetailReportDto> GetOrderDetailReport()
        {
            // 1. flatten the Orders and OrderItems into an tuple
            var flattenedOrderItems = _orders.SelectMany(
                order => order.Items,
                (order, item) => new 
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }
            );

            // 2.join the flattened list with the Products list to get the names and prices
           return flattenedOrderItems.Join(
                _products,
                flatItem => flatItem.ProductId,
                product => product.Id,      
                (flatItem, product) => new OrderDetailReportDto
                (
                    CustomerId: flatItem.CustomerId,
                    OrderId: flatItem.OrderId,
                    ProductName: product.Name,
                    Quantity: flatItem.Quantity,
                    UnitPrice: product.Price,
                    TotalPrice: flatItem.Quantity * product.Price
                )
            );
        }

        //challenge 6
        public decimal? CalculateTotalCompletedSales()
        {
            return _orders.Where(o => o.Status == OrderStatus.Completed)
                   .Select(o => o.CalculateSubtotal())
                   .Aggregate(0m, (curr, next) => curr + next);
        }

        //challenge 7
        public string? OrderItemTextSummary()
        {
            return _products.Aggregate("", (currText, nextItem) =>
            currText + $"{nextItem.Name} x {nextItem.QuantityInStock}, ").TrimEnd(',', ' ');
            // the TrimEnd for removing the comma and space at the end of the string
        }


        //check about making the IEnumerable<(string customerName, ....)> to IEnumerable<record>
        //check about using yield return for all these reportes
        //challenge 8
        public IEnumerable<CustomerSpendingRanking> CustomersRankedBySpending()
        {
            var completedOrders = _orders.Where(order => order.Status == OrderStatus.Completed);

            return _customers.GroupJoin(
                completedOrders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, customerOrders) => new CustomerSpendingRanking
                (
                    CustomerName: customer.FullName,
                    CompletedOrdersCount: customerOrders.Count(),
                    TotalSpend: customerOrders.Sum(order => order.CalculateSubtotal()),
                    AvgOrderValue: customerOrders.Any() ? customerOrders.Average(order => order.CalculateSubtotal()) : 0m
                )).OrderByDescending(summary => summary.TotalSpend);
        }

        //challenge 9
        public IEnumerable<BestSellingProductSummary> BestSellingProducts()
        {
            var completedOrders = _orders.Where(order => order.Status == OrderStatus.Completed);

            var ProductIdResult = completedOrders.SelectMany(order => order.Items)
                                  .GroupBy(product => product.ProductId)
                                  .Select(productGroup => (
                                            productId: productGroup.Key,
                                            quantitySold: productGroup.Sum(order => order.Quantity),
                                            totalSales: productGroup.Sum(order => order.UnitPrice)
                                           ));
            
            //nullable problem here
            return ProductIdResult.Join(
                _products,
                summary => summary.productId,
                product => product.Id,
                (summary, product) => new BestSellingProductSummary(
                    ProductName: product.Name,
                    QuantitySold: summary.quantitySold,
                    TotalSales: summary.totalSales
                )).OrderByDescending(result => result.QuantitySold)
                  .ThenByDescending(result => result.TotalSales);
        }

        //challenge 10
        public IEnumerable<Customer> CustomersWithNoOrders()
        {
            return _customers.GroupJoin(
                _orders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, orders) => new
                {
                    customer,
                    orders
                }
                ).Where(result => !result.orders.Any())
                 .Select(result => result.customer);
            
            
            //.Any() solution
            //return _customers.Where(customer => !_orders.Any(order => order.CustomerId == customer.Id));
        }

        //challenge 11
        public MostValuableOrderSummary? MostValuableOrder()
        {
            var completedOrders = _orders.Where(order => order.Status == OrderStatus.Completed);

            //join the customerId to return the whole customer object
            return _customers.Join(
                _orders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, order) => new MostValuableOrderSummary (
                    CustomerName: customer.FullName,
                    Subtotal: order.CalculateSubtotal(),
                    FinalTotal: order.CalculateFinalTotal(customer),
                    Discount: customer.GetDiscountPercentage()
                )).OrderByDescending(result => result.FinalTotal)
                  .FirstOrDefault();
        }


        //challenge 12
        public IEnumerable<MonthlySalesSummary> GetMonthlySalesReport()
        {
            return _orders.Where(order => order.Status == OrderStatus.Completed)
                          .Join(
                               _customers,
                               order => order.CustomerId,
                               customer => customer.Id,
                               (order, customer) => new { Order = order, Customer = customer }
                            )
                          .GroupBy(joinedData => new
                           {
                                Year = joinedData.Order.OrderDate.Year,
                                Month = joinedData.Order.OrderDate.Month
                           })
                          .Select(group => new MonthlySalesSummary(
                              group.Key.Year,
                              group.Key.Month,
                              group.Count(),
                              group.Sum(joinedData => joinedData.Order.CalculateFinalTotal(joinedData.Customer)),
                              group.Any() ? group.Average(joinedData => joinedData.Order.CalculateFinalTotal(joinedData.Customer)) : 0m
                             ))
                          .OrderBy(summary => summary.Year)
                          .ThenBy(summary => summary.Month);
        }


    }
}
