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
                //inform the user that nothing changes
                Console.WriteLine("invalid quantity");
            }

            this.QuantityInStock -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("invalid quantity");
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
            name = name.Trim();

            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("INVALID NAME: IS_NULL_OR_EMPTY VALIDATION FAILED");
            }

            Name = name;


            category = category.Trim();

            if (string.IsNullOrEmpty(category))
            {
                throw new Exception("INVALID CATEGORY: IS_NULL_OR_EMPTY VALIDATION FAILED");
            }

            Category = category;


            if (price <= 0)
            {
                throw new Exception("INVALID PRICE: MUST BE GREATER THAN ZERO");
            }

            Price = price;


            if (quantity < 0)
            {
                throw new Exception("INVALID QUANTITY: MUST BE NOT NEGATIVE");
            }

            QuantityInStock = quantity;
        }
    }
}

