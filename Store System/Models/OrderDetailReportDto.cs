using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class OrderDetailReportDto
    {
        public string CustomerId { get; set; }
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public OrderDetailReportDto(string customerId, string orderId, string productName, int quantity, decimal unitPrice, decimal totalPrice)
        {
            CustomerId = customerId;
            OrderId = orderId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = totalPrice;
        }

        public OrderDetailReportDto() { }
    }
}
