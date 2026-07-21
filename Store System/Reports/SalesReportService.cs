using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Store_System.Reports
{
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

        public IEnumerable<(string Name, string Category, decimal Price, StockStatus stockStatus)> GetProductProjection()
        {
            //in this solution no need to build a whole new struct or class
            return _products.Select(p => (
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

        public IEnumerable<(string CategoryName, int ProductCount, int TotalUnits, decimal AveragePrice, string MostExpensive, string LeastExpensive)>CagtegoryStatistics()
        {
            return _products.GroupBy(p => p.Category)
                            .Select(g => (
                                CategoryName: g.Key,
                                ProductCount: g.Count(),
                                TotalUnits: g.Sum(g => g.QuantityInStock),
                                AvaragePrice: g.Average(p => p.Price),
                                MostExpensice: g.MaxBy(p => p.Price).Name,
                                LeastExpensice: g.MinBy(p => p.Price).Name
                                ));
        }
        
        public IEnumerable<(string OrderId, string CustomerName, CustomerType CustomreType, DateTime OrderDate, OrderStatus OrderStatus, decimal OrderTotal)> GetCustomerOrderReport()
        {
            var CustomerOrderJoin = _orders.Join(
                _customers,
                o => o.CustomerId,
                c => c.Id,
                (order, customer) => (
                    orderId : order.Id,
                    customerName : customer.FullName,
                    customerType : customer.CustomerType,
                    orderDate : order.OrderDate,
                    orderStatus : order.Status,
                    orderTotal : order.CalculateSubtotal()
                ));

            return CustomerOrderJoin;
        }

        public IEnumerable<OrderDetailReportDto> GetOrderDetailReport()
        {
            // 1. Flatten the Orders and OrderItems into an intermediate anonymous type (or Tuple)
            var flattenedOrderItems = _orders.SelectMany(
                order => order.Items, // The collection to flatten
                (order, item) => new       // The projection that keeps the parent data
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }
            );

            // 2. Join the flattened list with the Products list to get the names and prices
            var detailedReport = flattenedOrderItems.Join(
                _products,
                flatItem => flatItem.ProductId, // Outer key
                product => product.Id,          // Inner key
                (flatItem, product) => new OrderDetailReportDto
                {
                    CustomerId = flatItem.CustomerId,
                    OrderId = flatItem.OrderId,
                    ProductName = product.Name,
                    Quantity = flatItem.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = flatItem.Quantity * product.Price
                }
            );

            return detailedReport;
        }

        public decimal CalculateTotalCompletedSales()
        {
            return _orders.Where(o => o.Status == OrderStatus.Completed)
                   .Select(o => o.CalculateSubtotal())
                   .Aggregate(0m, (curr, next) => curr + next);
        }

        public string OrderItemTextSummary()
        {
            return _products.Aggregate("", (currText, nextItem) =>
            currText + $"{nextItem.Name} x {nextItem.QuantityInStock}, ").TrimEnd(',', ' ');
            // the TrimEnd for removing the comma and space at the end of the string
        }


        //check about making the IEnumerable<(string customerName, ....)> to IEnumerable<record>
        //check about using yield return for all these reportes
        public IEnumerable<(string customerName, int completedOrdersCount, decimal totalSpend, decimal avgOrderValue)> CustomersRankedBySpending()
        {
            var completedOrders = _orders.Where(order => order.Status == OrderStatus.Completed);

            return _customers.GroupJoin(
                completedOrders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, customerOrders) =>
                (
                    customerName: customer.FullName,
                    completedOrderCount: customerOrders.Count(),
                    totalSpend: customerOrders.Sum(order => order.CalculateSubtotal()),
                    avgOrderValue: customerOrders.Any() ? customerOrders.Average(order => order.CalculateSubtotal()) : 0m
                )).OrderByDescending(summary => summary.totalSpend);
        }

        public IEnumerable<(string productName, int quantitySold, decimal totalSales)> BestSellingProducts()
        {
            var completedOrders = _orders.Where(order => order.Status == OrderStatus.Completed);

            var ProductIdResult = completedOrders?.SelectMany(order => order.Items)
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
                (summary, product) => (
                    productName: product.Name,
                    quantitySold: summary.quantitySold,
                    totalSales: summary.totalSales
                )).OrderByDescending(result => result.quantitySold)
                  .ThenByDescending(result => result.totalSales);
        }

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

        public (string customerName, decimal subtotal, decimal discount, decimal finaltotal) MostValuableOrder()
        {
            var completedOrders = _orders.Where(order => order.Status == OrderStatus.Completed);

            //join the customerId to return the whole customer object
            return _customers.Join(
                _orders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, order) => (
                    customerName: customer.FullName,
                    subtotal: order.CalculateSubtotal(),
                    finaltotal: order.CalculateFinalTotal(customer),
                    discount: customer.GetDiscountPercentage()
                )).OrderByDescending(result => result.finaltotal)
                  .FirstOrDefault();
        }

        public record MonthlySalesSummary(int Year, int Month, int OrderCount, decimal TotalSales,decimal AverageOrderValue);

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
