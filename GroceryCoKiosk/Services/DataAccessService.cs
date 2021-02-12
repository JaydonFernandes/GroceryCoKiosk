using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GroceryCoKiosk
{
    public class DataAccessService : IDataAccessService
    {
        private readonly ILogger<IDataAccessService> _log;
        private readonly IConfiguration _config;

        public DataAccessService(ILogger<IDataAccessService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public Hashtable GetProductCatalog()
        {
            Hashtable productCatalog = new Hashtable();
            List<Product> catalogItemList = new List<Product>();

            try
            {
                var path = _config.GetValue<string>("DataLocation");
                using (StreamReader r = new StreamReader(_config.GetValue<string>("DataLocation")))  //Getting product data from local json file set in appsettings.json.
                {
                    string json = r.ReadToEnd();
                    try
                    {
                        catalogItemList = JsonConvert.DeserializeObject<List<Product>>(json);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError("Could not deserialize json object to a valid Product object. {ExceptionType} {ExceptionMessage} {ExceptionStackTrace}", ex.GetType(), ex.Message, ex.StackTrace);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError("Unable to find product catalog file. {ExceptionType} {ExceptionMessage} {ExceptionStackTrace}", ex.GetType(), ex.Message, ex.StackTrace);
                throw;
            }

            foreach (Product catalogItem in catalogItemList)
            {
                if ( (catalogItem.Name != null) && (catalogItem.Price >= 0) && (catalogItem.Discount >= 0 && catalogItem.Discount <= catalogItem.Price))
                {
                    productCatalog.Add(catalogItem.Name, (Product)catalogItem);
                }
                else
                {
                    _log.LogWarning("Could not format item from catalog.");
                }
                
            }
            return productCatalog;
        }
    }
}
