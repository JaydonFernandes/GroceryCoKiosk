using GroceryCoKiosk.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GroceryCoKiosk
{
    public class ItemScanningService : IItemScanningService
    {

        private readonly ILogger<IItemScanningService> _log;
        private readonly IKioskPrinter _kioskPrinter;
        private readonly IDataAccessService _dataAccessService;

        public ItemScanningService(ILogger<IItemScanningService> log, IKioskPrinter kioskPrinter, IDataAccessService dataAccessService)
        {
            _log = log;
            _kioskPrinter = kioskPrinter;
            _dataAccessService = dataAccessService;
        }

        public List<Product> ScanItems(string[] checkoutItemArray)
        {
            _log.LogInformation("Scanning items.");
            checkoutItemArray = FormatItemArray(checkoutItemArray);
            Hashtable productCataglog = _dataAccessService.GetProductCatalog();
            List<Product> productsToCheckout = new List<Product>();

            foreach (string checkoutItem in checkoutItemArray)
            {
                if (productCataglog.ContainsKey(checkoutItem))
                {
                    productsToCheckout.Add((Product)productCataglog[checkoutItem]);
                    _log.LogInformation("Scanned item [{ScannedItem}].", checkoutItem);
                }
                else
                {
                    _log.LogWarning("Unable to find item [{UnknownItem}] in product catalog.",checkoutItem);
                    _kioskPrinter.PrintLineToConsole($"Unable to find item [{checkoutItem}] in product catalog.");
                }
            }
            _log.LogInformation("Scanning complete.");
            return productsToCheckout;
        }

        private string[] FormatItemArray(string[] itemList)
        {
            for (int i = 0; i < itemList.Length; i++)
            {
                itemList[i] = itemList[i].Trim();
            }
            return itemList.Where(item => (item != "")).ToArray();
        }
    }


}
