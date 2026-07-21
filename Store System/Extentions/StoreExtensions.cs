using Store_System.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Extentions
{
    static internal class StoreExtensions
    {

        //must call it from the StoreServices file
        public static decimal ApplyDiscount(this decimal amount, decimal discountPercentage)
        {
            if (amount < 0)
            {
                throw new Exception("AMOUNT INVALID: NEGATIVE AMOUNT");
            }

            if (discountPercentage < 0 || discountPercentage > 100)
            {
                throw new Exception("DISCOUNT PERCENTAGE INVALID: MUST BE BETWEEN 100 AND 0");
            }

            return amount - (amount * discountPercentage);
        }

        public static IEnumerable<Product> AvailableOnly(this IEnumerable<Product> products) =>
            products.Where(p => p.QuantityInStock > 0);

        public static IEnumerable<Product> InCategory(this IEnumerable<Product> products, string category) =>
            products.Where(p => p.Category == category);

        public static IEnumerable<Product> WithinPriceRange(this IEnumerable<Product> products,decimal minimumPrice,decimal maximumPrice)
        {
            if (minimumPrice >= maximumPrice)
            {
                throw new Exception("INVALID MIN/MAX PRICE: MAX_PRICE MUST BE GREATER THAN MIN_PRICE");
            }

            return products.Where(p => minimumPrice < p.Price && p.Price < maximumPrice);

            ;
        }

        // there is OrderBy implemented in the StoreServices at the end of method chain (page 9)
    }
}
