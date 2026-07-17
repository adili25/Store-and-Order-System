using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class Order
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.Now;
        public List<OrderItem> Items { get; private set; }
        public OrderStatus Status { get; private set; }


        public void AddItem(OrderItem item) => Items.Add(item);

        //i will merge the subtotal with the finaltotal method
        public decimal CalculateSubtotal() => Items.Sum(item => item.UnitPrice);
        public decimal CalculateFinalTotal(Customer customer)
        {
            decimal Subtotal = this.CalculateSubtotal();

            return Subtotal - (Subtotal * customer.GetDiscountPercentage());
        }

        public void CompleteOrder() => this.Status = OrderStatus.Completed;
        public void CancelOrder() => this.Status = OrderStatus.Cancelled;


        public Order(List<OrderItem> items, string customerId)
        {
            if (!items.Any())
            {
                throw new Exception("INVALID ITEMS: MSUT BE AT LEAST ONE ITME");
            }

            Items = items;

            Status = OrderStatus.Pending;

            CustomerId = customerId;

            
        }
    }
}
