using GroceryCoKiosk.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;

namespace GroceryCoKiosk
{

    public struct ProductReceipt
    {
        public decimal Price;
        public decimal Discount;
        public int Quantity;
        public decimal ItemTotal;
    }

    public class CheckoutService : ICheckoutService
    {
        private readonly ILogger<ICheckoutService> _log;
        private readonly IKioskPrinter _kioskPrinter;

        public CheckoutService(ILogger<ICheckoutService> log, IKioskPrinter kioskPrinter)
        {
            _log = log;
            _kioskPrinter = kioskPrinter;
        }

        public void Checkout(Order order)
        {
            if (order.Products.Count > 0)
            {
                _log.LogInformation("Starting Transaction.");
                Hashtable orderHash = HashOrder(order);
                _log.LogInformation("Order processed. Subtotal: ${OrderSubtotal}, Discount: ${OrderDiscount}, Total: ${OrderTotal}.", order.SubTotal, order.Discount, (order.SubTotal - order.Discount));

                _log.LogInformation("Printing receipt.");
                _kioskPrinter.PrintReceipt(orderHash);
                _log.LogInformation("Transaction complete.");
            }
            else
            {
                _log.LogWarning("Checkout attempted with empty order");
                _kioskPrinter.PrintLineToConsole("Unable to checkout empty order.");
            }
        }

            

        private Hashtable HashOrder(Order order)
        {
            Hashtable orderHash = new Hashtable();

            foreach (Product product in order.Products)
            {
                ProductReceipt itemReceipt;

                if (orderHash.ContainsKey(product.Name))
                {
                    itemReceipt = (ProductReceipt)orderHash[product.Name];
                    itemReceipt.Quantity++;
                    itemReceipt.ItemTotal = (itemReceipt.Quantity * (itemReceipt.Price - itemReceipt.Discount));
                    orderHash[product.Name] = itemReceipt;
                }
                else
                {
                    itemReceipt.Price = product.Price;
                    itemReceipt.Discount = product.Discount;
                    itemReceipt.Quantity = 1;
                    itemReceipt.ItemTotal = (itemReceipt.Quantity * (itemReceipt.Price - itemReceipt.Discount));
                    orderHash.Add(product.Name, itemReceipt);
                }
            }
            return orderHash;
        }
    }
}