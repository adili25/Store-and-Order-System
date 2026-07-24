using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class Product
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string Name { get; private set; }
        public string Category { get; private set; }
        public decimal Price { get; private set; }
        public int QuantityInStock { get; private set; }

        public bool IsAvailable(int requestedQuantity) => (QuantityInStock - requestedQuantity) > 0 ? true : false;

        public void ReduceStock(int quantity)
        {
            if (this.QuantityInStock - quantity < 0 || quantity <= 0)
            {
                throw new ArgumentOutOfRangeException("INVALID STOCK: MUST BE STOCK > 0 AND STOCK <= QUANTITY", nameof(quantity));
            }

            this.QuantityInStock -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("INVALID STOCK: MSUT BE STOCK > 0", nameof(quantity));
            }

            this.QuantityInStock += quantity;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($@"
==========Procduct==========
Id: {this.Id}
Name: {this.Name}
Category: {this.Category}
Price: {this.Price}
Quantity in stock: {this.QuantityInStock}
============================
");
        }


        public Product(string name, string category, decimal price, int quantity)
        {
            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("INVALID NAME: IS_NULL_OR_EMPTY VALIDATION FAILED", nameof(name));
            }

            Name = name;


            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("INVALID CATEGORY: IS_NULL_OR_EMPTY VALIDATION FAILED", nameof(category);
            }

            Category = category;


            if (price <= 0)
            {
                throw new ArgumentException("INVALID PRICE: MUST BE GREATER THAN ZERO", nameof(price));
            }

            Price = price;


            if (quantity < 0)
            {
                throw new ArgumentException("INVALID QUANTITY: MUST BE NOT NEGATIVE", nameof(quantity));
            }

            QuantityInStock = quantity;
        }
    }
}

