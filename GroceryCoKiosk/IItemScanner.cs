using System.Collections.Generic;

namespace GroceryCoKiosk
{
    public interface IItemScanner
    {
        List<Product> ScanItems(string[] itemList);
    }
}