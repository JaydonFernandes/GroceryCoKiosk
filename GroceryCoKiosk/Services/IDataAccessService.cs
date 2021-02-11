using System.Collections;

namespace GroceryCoKiosk
{
    public interface IDataAccessService
    {
        Hashtable GetProductCatalog();
    }
}