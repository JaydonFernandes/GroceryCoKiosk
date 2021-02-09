using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroceryCoKiosk
{
    class Program
    {
        static void Main(string[] args)
        {


            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ICheckoutService, CheckoutService>();
                    services.AddTransient<IItemScanner, ItemScanner>();
                })
                .UseSerilog()
                .Build();

            var itemScanner = ActivatorUtilities.CreateInstance<ItemScanner>(host.Services);
            var checkoutSvc = ActivatorUtilities.CreateInstance<CheckoutService>(host.Services);


            string items = File.ReadAllText(args[0]);

            List<Product> products = new List<Product>(); 

            try
            {
                products = itemScanner.ScanItems(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with scanning scanning items. Please contact GroceryCo Kiosk support.");
            }

            if (products.Count > 0)
            {
                Order order = new Order(products);
                checkoutSvc.Checkout(order);
            }
            
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
