using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;

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


        public IEnumerable<Product> GetDynamicProductSearch(string name = null, string category = null, decimal? minPrice = null)
        {
            IEnumerable<Product> query = _products;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name == name);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (!(minPrice == null))
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            query = query.Where(p => p.QuantityInStock > 0);

            return query;
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
        
    }
}
