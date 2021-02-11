using System.Collections.Generic;

namespace GroceryCoKiosk
{
    public interface IItemScanningService
    {
        List<Product> ScanItems(string[] itemList);
    }
}