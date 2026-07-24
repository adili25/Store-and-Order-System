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

        //build display info for the order class (case 7 in the main)
        public void DisplayInfo()
        {
            Console.WriteLine(@$"
Id: {Id}
Order date and time: {OrderDate.ToString("dd/MM/yyyy HH:mm")}
CustomerId: {CustomerId}
Order status: {Status}
Items:
");
            foreach(var item in Items)
            {
                item.DisplayInfo();
            }
        }

        public void AddItem(OrderItem item)
        {
            if (this.Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("INVALID ADD ITEM: THE ORDER IS COMPLETED");
            }

            Items.Add(item);
        }

        public decimal CalculateSubtotal() => Items.Sum(item => item.TotalPrice);

        public decimal CalculateFinalTotal(Customer customer)
        {
            decimal Subtotal = this.CalculateSubtotal();

            return Subtotal - (Subtotal * customer.GetDiscountPercentage());
        }

        public void CompleteOrder()
        {
            if (!Items.Any())
            {
                throw new InvalidOperationException("INVALID COMPLETE ORDER: MUST BE AT LEAST ONE ITEM");
            }

            if (this.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("INVALID COMPELTE ORDER: ORDER STATUS IS CANCELLED");
            }

            this.Status = OrderStatus.Completed;

        }

        public void CancelOrder()
        {
            if (this.Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("INVALID CANCEL ORDER: ORDER COMPLETED");
            }

            this.Status = OrderStatus.Cancelled;
        }

        //to create an empty order, then add into it the items
        public Order(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentException("INVALID CREATE ORDER: CUSTOMER ID IS_NULL_OR_WHITESPACE", nameof(customerId));
            }
            CustomerId = customerId;

            Status = OrderStatus.Pending;

            Items = new List<OrderItem>();
        }

        public Order(List<OrderItem> items, string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentException("INVALID CREATE ORDER: CUSTOMER ID IS_NULL_OR_WHITESPACE", nameof(customerId));
            }
            CustomerId = customerId;

            if (!items.Any())
            {
                throw new ArgumentException("INVALID CREATE ORDER: NO ITEM IN ITEMS LIST", nameof(items));
            }
            Items = items;

            Status = OrderStatus.Completed;
        }
    }
}
