using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GroceryCoKiosk.Views
{
    public class KioskPrinter : IKioskPrinter
    {
        public void PrintReceipt(Hashtable orderHash)
        {
            DateTime localDate = DateTime.Now;
            decimal subtotal = 0;
            decimal discounts = 0;
            int itemCount = 0;

            Console.WriteLine("");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine($"{localDate.DayOfWeek} {localDate.ToString("MMMM")} {localDate.Day}, {localDate.Year}\t{localDate.ToString("h:mm tt")}");
            Console.WriteLine("");
            foreach (DictionaryEntry productKeyValuePair in orderHash)
            {
                ProductReceipt productItemReceipt = (ProductReceipt)productKeyValuePair.Value;
                string productName = (string)productKeyValuePair.Key;

                subtotal += (productItemReceipt.Price * productItemReceipt.Quantity);
                discounts += (productItemReceipt.Discount * productItemReceipt.Quantity);
                itemCount += productItemReceipt.Quantity;

                Console.WriteLine(productName);
                Console.WriteLine($"\tRegular price: ${productItemReceipt.Price}");
                if (productItemReceipt.Discount > 0)
                {
                    Console.WriteLine($"\tPrice after discount: ${productItemReceipt.Price - productItemReceipt.Discount}");
                }
                Console.WriteLine($"\tQuantity: {productItemReceipt.Quantity}");
                Console.WriteLine($"\tItem total: ${productItemReceipt.ItemTotal}");
                Console.WriteLine("");

            }
            Console.WriteLine("======================================");
            Console.WriteLine($"Subtotal ({itemCount} items):\t\t ${subtotal}");
            if (discounts > 0)
            {
                Console.WriteLine($"Discount:\t\t\t-${discounts}");
            }
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"Total:\t\t\t\t ${subtotal - discounts}");
            Console.WriteLine("======================================");
            Console.WriteLine("");
            Console.WriteLine(" Thank you for shopping at GroceryCo. ");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
        }

        // Wrapped Console.WriteLine for UI separation and testability.
        public void PrintLineToConsole(string line)
        {
            Console.WriteLine(line);
        }
    }
}
