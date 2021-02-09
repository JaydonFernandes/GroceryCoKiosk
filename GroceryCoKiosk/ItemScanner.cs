using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GroceryCoKiosk
{
    public class ItemScanner : IItemScanner
    {

        private readonly ILogger<ItemScanner> _log;

        public ItemScanner(ILogger<ItemScanner> log)
        {
            _log = log;
        }



        public List<Product> ScanItems(string items)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string productsFile = Path.Combine(projectDirectory, "products.json");
            string productsText = File.ReadAllText(productsFile);
            dynamic productData = JsonConvert.DeserializeObject<dynamic>(productsText);


            string[] itemList = items.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            List<Product> products = new List<Product>();

            foreach (string item in itemList)
            {
                bool itemFound = false;
                foreach (var product in productData)
                {

                    if ( (item == (string)product.name) )
                    {
                        try
                        {
                            Product scannedItem = new Product((string)product.name, (decimal)product.price, (decimal)product.discount);
                            _log.LogInformation($"Item scanned: {item}");
                            products.Add(scannedItem);
                            itemFound = true;
                        }
                        catch (Exception ex)
                        {
                            _log.LogError("{Type} when scanning item: {Item}. {Message} {StackTrace}", ex.GetType(), item, ex.Message, ex.StackTrace);
                            throw;
                        }

                    }
                }
                if (!itemFound)
                {
                    Exception ex = new KeyNotFoundException($"Unable to match item: {item} in product database.");
                    _log.LogError("{Type} when scanning item: {Item}. {Message}", ex.GetType(), item, ex.Message);
                    throw ex;
                }
            }

            return products;
        }
    }
}
