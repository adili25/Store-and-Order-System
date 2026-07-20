using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{

    public enum StockStatus
    {
        OutOfStock,
        Available
    }
    internal readonly struct ProductProjectionDto
    {
        public string Name { get; }
        public string Category { get; }
        public decimal Price { get; }
        public StockStatus StockStatus { get; }

        public ProductProjectionDto(string name, string category, decimal price, StockStatus stockstatus)
        {
            Name = name;
            Category = category;
            Price = price;
            StockStatus = stockstatus;
        }

    }
}
