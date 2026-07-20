using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class Order
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public DateTime OrderDate { get; private set; } = DateTime.Now;
        public string CustomerId { get; private set; }
        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();
        public OrderStatus Status { get; private set; }


        public void AddItem(OrderItem item)
        {
            if (this.Status == OrderStatus.Completed)
            {
                Console.WriteLine("invalid addItem: can't add item to completed order");
                return;
            }

            Items.Add(item);
        }

        public decimal CalculateSubtotal() => Items.Sum(item => item.UnitPrice);

        public decimal CalculateFinalTotal(Customer customer)
        {
            decimal Subtotal = this.CalculateSubtotal();

            return Subtotal - (Subtotal * customer.GetDiscountPercentage());
        }

        public void CompleteOrder()
        {
            if (!Items.Any())
            {
                throw new Exception("INVALID ORDER STATUS: MUST BE ATLEAST ONE ITEM");
            }

            if (this.Status == OrderStatus.Cancelled)
            {
                Console.WriteLine("invalid order status: can't change status of cancelled order");
                return;
            }

            this.Status = OrderStatus.Completed;

        }

        public void CancelOrder()
        {
            if (this.Status == OrderStatus.Completed)
            {
                Console.WriteLine("the current order is completed, you cannot cancel it");
                return;
            }

            this.Status = OrderStatus.Cancelled;
        }

        //to create an empty order, then add into it the items
        public Order(string customerId)
        {
            CustomerId = customerId;

            Status = OrderStatus.Pending;

            Items = new List<OrderItem>();
        }

        public Order(List<OrderItem> items, string customerId)
        {
            CustomerId = customerId;

            Status = OrderStatus.Completed;

            Items = items;
        }
    }
}
