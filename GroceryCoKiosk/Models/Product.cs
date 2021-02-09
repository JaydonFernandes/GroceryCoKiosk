using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryCoKiosk
{

    public class Product
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public decimal Discount { get; private set; }

        public Product(string name, decimal price, decimal discount)
        {
            Name = name;
            Price = price;
            Discount = discount;
        }
    }
}
