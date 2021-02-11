using GroceryCoKiosk.Views;
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
            
            //Initial setup for appsettings.json, DI and Logger
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            SetupLogger(builder);
            var host = SetupHost();

            var itemScanner = ActivatorUtilities.CreateInstance<ItemScanningService>(host.Services);
            var checkoutService = ActivatorUtilities.CreateInstance<CheckoutService>(host.Services);
            var kioskPrinter = ActivatorUtilities.CreateInstance<KioskPrinter>(host.Services);

            if (args.Length < 1)
            {
                kioskPrinter.PrintLineToConsole("Please pass full path to order file as a command line argument.");
                Log.Logger.Error("No input file provided.");
                return;
            }

            if (!File.Exists(args[0]))
            {
                kioskPrinter.PrintLineToConsole($"Could not find input file [{args[0]}].");
                Log.Logger.Error("Could not find input file [{InputFile}].", args[0]);
                return;
            }

            string[] fileContents = File.ReadAllText(args[0]).Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            List<Product> productsToCheckout = new List<Product>();
            try
            {
                productsToCheckout = itemScanner.ScanItems(fileContents);
            }
            catch (Exception)
            {
                kioskPrinter.PrintLineToConsole("Error with scanning items. Please contact GroceryCo Kiosk support.");
            }

            if (productsToCheckout.Count > 0)
            {
                Order order = new Order(productsToCheckout);
                checkoutService.Checkout(order);
            }
            else
            {
                Log.Logger.Warning("No valid items to checkout.");
                kioskPrinter.PrintLineToConsole("No valid items to checkout.");
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
                .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Logger.Information("Application Starting");
        }

        static IHost SetupHost()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IKioskPrinter, KioskPrinter>();

                    services.AddTransient<ICheckoutService, CheckoutService>();
                    services.AddTransient<IItemScanningService, ItemScanningService>();
                    services.AddTransient<IDataAccessService, DataAccessService>();
                })
                .UseSerilog()
                .Build();

            return host;
        }
    }
}
