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

            PrintLineToConsole("");
            PrintLineToConsole("++++++++++++++++++++++++++++++++++++++");
            PrintLineToConsole($"{localDate.DayOfWeek} {localDate.ToString("MMMM")} {localDate.Day}, {localDate.Year}    {localDate.ToString("h:mm tt")}");
            PrintLineToConsole("");
            foreach (DictionaryEntry productKeyValuePair in orderHash)
            {
                ProductReceipt productItemReceipt = (ProductReceipt)productKeyValuePair.Value;
                string productName = (string)productKeyValuePair.Key;

                subtotal += (productItemReceipt.Price * productItemReceipt.Quantity);
                discounts += (productItemReceipt.Discount * productItemReceipt.Quantity);
                itemCount += productItemReceipt.Quantity;

                PrintLineToConsole(productName);
                PrintLineToConsole($"\tRegular price: ${productItemReceipt.Price}");
                if (productItemReceipt.Discount > 0)
                {
                    PrintLineToConsole($"\tPrice after discount: ${productItemReceipt.Price - productItemReceipt.Discount}");
                }
                PrintLineToConsole($"\tQuantity: {productItemReceipt.Quantity}");
                PrintLineToConsole($"\tItem total: ${productItemReceipt.ItemTotal}");
                PrintLineToConsole("");

            }
            PrintLineToConsole("======================================");
            PrintLineToConsole($"Subtotal ({itemCount} items):\t\t ${subtotal}");
            if (discounts > 0)
            {
                PrintLineToConsole($"Discount:\t\t\t-${discounts}");
            }
            PrintLineToConsole("--------------------------------------");
            PrintLineToConsole($"Total:\t\t\t\t ${subtotal - discounts}");
            PrintLineToConsole("======================================");
            PrintLineToConsole("");
            PrintLineToConsole(" Thank you for shopping at GroceryCo. ");
            PrintLineToConsole("++++++++++++++++++++++++++++++++++++++");
            PrintLineToConsole("");
        }

        // Wrapped Console.WriteLine for UI separation and testability.
        public void PrintLineToConsole(string line)
        {
            Console.WriteLine(line);
        }
    }
}
