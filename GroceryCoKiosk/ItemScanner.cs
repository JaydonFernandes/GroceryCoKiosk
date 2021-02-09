using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GroceryCoKiosk
{
    public class ItemScanner : IItemScanner
    {

        private readonly ILogger<ItemScanner> _log;
        private readonly IConfiguration _config;

        public ItemScanner(ILogger<ItemScanner> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }


        public List<Product> ScanItems(string[] itemList)
        {
            _log.LogInformation("Scanning items.");
            itemList = CleanInput(itemList);
            List<Product> productData = GetProducts();

            List<Product> products = new List<Product>();

            foreach (string item in itemList)
            {
                bool itemFound = false;
                foreach (var product in productData)
                {
                    if ((item == product.Name))
                    {
                        try
                        {
                            Product scannedItem = product;//////////////
                            _log.LogInformation("Item scanned: {item}", item);
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
                    Exception ex = new KeyNotFoundException($"Unable to match item: [{item}] in product database.");
                    _log.LogError("{Type} when scanning item: {Item}. {Message}", ex.GetType(), item, ex.Message);
                    throw ex;
                }
            }
            _log.LogInformation("Scanning complete.");
            return products;
        }

        private List<Product> GetProducts()
        {
            List<Product> items = new List<Product>();
            using (StreamReader r = new StreamReader(_config.GetValue<string>("DataLocation")))  //Getting product data from local json file set in appsettings.json.
            {
                string json = r.ReadToEnd();

                try
                {
                    items = JsonConvert.DeserializeObject<List<Product>>(json);
                }
                catch (Exception)
                {
                    _log.LogError("Could not deserialize json object to a valid Product object.");
                    throw;
                }

            }
            return items;
        }

        private string[] CleanInput(string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = input[i].Trim();
            }
            return input.Where(o => (o != "")).ToArray();
        }
    }


}
