using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GroceryCoKiosk
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            SetupLogger(builder);
            var host = SetupHost();

            var itemScanner = ActivatorUtilities.CreateInstance<ItemScanner>(host.Services);
            var checkoutSvc = ActivatorUtilities.CreateInstance<CheckoutService>(host.Services);

            string[] fileContents = File.ReadAllText(args[0]).Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            List<Product> products = new List<Product>(); 

            try
            {
                products = itemScanner.ScanItems(fileContents);
            }
            catch (Exception)
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

        static void SetupLogger(ConfigurationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");
        }

        static IHost SetupHost()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ICheckoutService, CheckoutService>();
                    services.AddTransient<IItemScanner, ItemScanner>();
                })
                .UseSerilog()
                .Build();

            return host;
        }
    }
}
