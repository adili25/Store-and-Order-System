using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class OrderItem
    {
        public string ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice => this.Quantity * this.UnitPrice;

        public void DisplayInfo()
        {
            Console.WriteLine($@"
------------
Product Id: {ProductId}
Quantity: {Quantity}
Price: {UnitPrice}
Quantity * Price: {TotalPrice}
------------
");
        }
        //the constructor here automatilly lock in the UnitPrice
        public OrderItem(string productId, int quantity, decimal unitPrice)
        {
            ProductId = productId;

            if (quantity < 0)
            {
                throw new ArgumentException("INVALID QUANTITY: MUST BE GRATER THAN ZERO", nameof(quantity));
            }

            Quantity = quantity;


            if (unitPrice < 0)
            {
                throw new ArgumentException("INVALID PRICE: MUST BE GRATER THAN ZERO", nameof(unitPrice));
            }

            UnitPrice = unitPrice;
        }
    }
}
