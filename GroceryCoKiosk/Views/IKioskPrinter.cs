using System.Collections;

namespace GroceryCoKiosk.Views
{
    public interface IKioskPrinter
    {
        void PrintLineToConsole(string message);
        void PrintReceipt(Hashtable orderHash);
    }
}