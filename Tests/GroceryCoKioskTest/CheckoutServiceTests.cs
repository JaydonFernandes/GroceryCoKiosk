using System;
using System.Collections;
using System.Collections.Generic;
using GroceryCoKiosk;
using GroceryCoKiosk.Views;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GroceryCoKioskTest
{
    [TestClass]
    public class CheckoutServiceTests
    {
        private readonly CheckoutService _checkoutService;

        private readonly Mock<ILogger<CheckoutService>> _logMock = new Mock<ILogger<CheckoutService>>();
        private readonly Mock<IKioskPrinter> _kioskPrinterMock = new Mock<IKioskPrinter>();

        public CheckoutServiceTests()
        {
            _checkoutService = new CheckoutService(_logMock.Object, _kioskPrinterMock.Object);
        }

        [TestMethod]
        public void CheckoutOrderWithSingleItemTest()
        {
            //--Arrange
            Product sampleItem = new Product("Apple", (decimal)0.75, (decimal)0.25);

            Order order = new Order(new List<Product>() {
                sampleItem
            });

            ProductReceipt productReceipt = new ProductReceipt();
            productReceipt.Discount = sampleItem.Discount;
            productReceipt.Price = sampleItem.Price;
            productReceipt.Quantity = 1;
            productReceipt.ItemTotal = (productReceipt.Quantity * (productReceipt.Price - productReceipt.Discount));

            Hashtable expectedParameter = new Hashtable();
            expectedParameter.Add(sampleItem.Name, productReceipt);

            //--Act
            _checkoutService.Checkout(order);

            //--Assert
            _kioskPrinterMock.Verify(x => x.PrintReceipt(expectedParameter), Times.Once);
        }

        [TestMethod]
        public void CheckoutOrderWithMultipleItemsTest()
        {
            //--Arrange
            Product sampleItem1 = new Product("Apple", (decimal)0.75, (decimal)0.25);
            Product sampleItem2 = new Product("Banana", (decimal)1.00, (decimal)0.00);

            Order order = new Order(new List<Product>() {
                sampleItem1,
                sampleItem2
            });

            ProductReceipt productReceipt1 = new ProductReceipt();
            productReceipt1.Discount = sampleItem1.Discount;
            productReceipt1.Price = sampleItem1.Price;
            productReceipt1.Quantity = 1;
            productReceipt1.ItemTotal = (productReceipt1.Quantity * (productReceipt1.Price - productReceipt1.Discount));

            ProductReceipt productReceipt2 = new ProductReceipt();
            productReceipt2.Discount = sampleItem2.Discount;
            productReceipt2.Price = sampleItem2.Price;
            productReceipt2.Quantity = 1;
            productReceipt2.ItemTotal = (productReceipt2.Quantity * (productReceipt2.Price - productReceipt2.Discount));

            Hashtable expectedParameter = new Hashtable();
            expectedParameter.Add(sampleItem1.Name, productReceipt1);
            expectedParameter.Add(sampleItem2.Name, productReceipt2);

            //--Act
            _checkoutService.Checkout(order);

            //--Assert
            _kioskPrinterMock.Verify(x => x.PrintReceipt(expectedParameter), Times.Once);
        }

        [TestMethod]
        public void CheckoutOrderWithRepeatingItemsTest()
        {
            //--Arrange
            Product sampleItem1 = new Product("Apple", (decimal)0.75, (decimal)0.25);
            Product sampleItem2 = new Product("Apple", (decimal)0.75, (decimal)0.25);

            Order order = new Order(new List<Product>() {
                sampleItem1,
                sampleItem2
            });

            ProductReceipt productReceipt1 = new ProductReceipt();
            productReceipt1.Discount = sampleItem1.Discount;
            productReceipt1.Price = sampleItem1.Price;
            productReceipt1.Quantity = 2;
            productReceipt1.ItemTotal = (productReceipt1.Quantity * (productReceipt1.Price - productReceipt1.Discount));

            Hashtable expectedParameter = new Hashtable();
            expectedParameter.Add(sampleItem1.Name, productReceipt1);

            //--Act
            _checkoutService.Checkout(order);

            //--Assert
            _kioskPrinterMock.Verify(x => x.PrintReceipt(expectedParameter), Times.Once);
        }

        [TestMethod]
        public void CheckoutOrderWithNoItemsTest()
        {
            //--Arrange

            Order order = new Order(new List<Product>() {});

            //--Act
            _checkoutService.Checkout(order);

            //--Assert
            _kioskPrinterMock.Verify(x => x.PrintReceipt(It.IsAny<Hashtable>()), Times.Never);
            _kioskPrinterMock.Verify(x => x.PrintLineToConsole("Unable to checkout empty order."), Times.Once);
        }
    }
}
