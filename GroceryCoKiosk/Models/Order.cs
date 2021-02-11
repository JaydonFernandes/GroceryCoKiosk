using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryCoKiosk
{
    public class Order
    {
        public List<Product> Products { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal TotalDiscount { get; private set; }

        public Order(List<Product> products)
        {
            Products = products;
            foreach (Product product in products)
            {
                SubTotal += product.Price;
                TotalDiscount += product.Discount;
            }
        }
    }
}