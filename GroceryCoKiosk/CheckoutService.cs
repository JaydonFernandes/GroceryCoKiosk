using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;

namespace GroceryCoKiosk
{

    struct ItemReceipt
    {
        public decimal Price;
        public decimal Discount;
        public int Quantity;
        public decimal ItemTotal;
    }

    public class CheckoutService : ICheckoutService
    {
        private readonly ILogger<CheckoutService> _log;
        private readonly IConfiguration _config;

        public CheckoutService(ILogger<CheckoutService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public void Checkout(Order order)
        {
            _log.LogInformation("Starting Transaction.");
            Hashtable productHash = new Hashtable();

            foreach (Product product in order.Products)
            {
                if (productHash.ContainsKey(product.Name))
                {
                    ItemReceipt itemReceipt = (ItemReceipt) productHash[product.Name];
                    itemReceipt.Price = product.Price;
                    itemReceipt.Quantity++;
                    itemReceipt.Discount += product.Discount;
                    itemReceipt.ItemTotal = (itemReceipt.Quantity * itemReceipt.Price) - itemReceipt.Discount;

                    productHash[product.Name] = itemReceipt;
                }
                else
                {
                    ItemReceipt productReceipt;
                    productReceipt.Price = product.Price;
                    productReceipt.Discount = product.Discount;
                    productReceipt.Quantity = 1;
                    productReceipt.ItemTotal = (product.Price - product.Discount);

                    productHash.Add(product.Name, productReceipt);
                }
            }
            _log.LogInformation("Order total: ${OrderTotal}.", order.SubTotal-order.Discount);
            _log.LogInformation("Printing receipt.");
            PrintItemizedReceipt(productHash);
            _log.LogInformation("Transaction complete.");
        }

        private void PrintItemizedReceipt(Hashtable productHash)
        {
            DateTime localDate = DateTime.Now;
            decimal subtotal = 0;
            decimal discounts = 0;
            int itemCount = 0;

            Console.WriteLine();
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine($"{localDate.DayOfWeek} {localDate.ToString("MMMM")} {localDate.Day}, {localDate.Year}    {localDate.ToString("h:mm tt")}");
            Console.WriteLine();
            foreach (DictionaryEntry pair in productHash)
            {
                ItemReceipt item = (ItemReceipt)pair.Value;
                subtotal += (item.Price * item.Quantity);
                discounts += (item.Discount);
                itemCount += item.Quantity;

                Console.WriteLine(pair.Key);
                Console.WriteLine($"        Price: ${item.Price}");
                Console.WriteLine($"        Quantity: {item.Quantity}");
                if (item.Discount > 0)
                {
                    Console.WriteLine($"        Discount: {item.Discount}");
                }

                Console.WriteLine($"        Item Total: ${item.ItemTotal}");
                Console.WriteLine();

            }
            Console.WriteLine("======================================");
            Console.WriteLine($"Subtotal ({itemCount} items):        ${subtotal}");
            if (discounts > 0)
            {
                Console.WriteLine($"Discount:                 -${discounts}");
            }
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"Total:                     ${subtotal - discounts}");
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine(" Thank you for shopping at GroceryCo. ");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine();
        }

    }
}
