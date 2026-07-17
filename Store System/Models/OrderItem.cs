using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class OrderItem
    {
        public string ProductId { get; private set; } = Guid.NewGuid().ToString();
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        //TotalPrice is a computed prop, TotalPrice() is a method
        public decimal TotalPrice => this.Quantity * this.UnitPrice;


        //the constructor here automatilly lock in the UnitPrice
        public OrderItem(int quantity, decimal unitPrice)
        {
            if (quantity < 0)
            {
                throw new Exception("INVALID QUANTITY: MUST BE GRATER THAN ZERO");
            }

            Quantity = quantity;


            if (UnitPrice < 0)
            {
                throw new Exception("INVALID PRICE: MUST BE GRATER THAN ZERO");
            }

            UnitPrice = unitPrice;
        }
    }
}
