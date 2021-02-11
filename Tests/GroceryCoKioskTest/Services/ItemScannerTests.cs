using GroceryCoKiosk;
using GroceryCoKiosk.Views;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using System;

namespace GroceryCoKioskTest
{
    [TestClass]
    public class ItemScannerTests
    {
        private readonly ItemScanningService _itemScanner;
        private static readonly Mock<IDataAccessService> _dataAccessServiceMock = new Mock<IDataAccessService>();
        private readonly Mock<ILogger<IItemScanningService>> _logMock = new Mock<ILogger<IItemScanningService>>();
        private readonly Mock<IKioskPrinter> _kioskPrinterMock = new Mock<IKioskPrinter>();

        static Hashtable fakeProductCatalog;

        public ItemScannerTests()
        {
            _itemScanner = new ItemScanningService(_logMock.Object, _kioskPrinterMock.Object, _dataAccessServiceMock.Object);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            fakeProductCatalog = new Hashtable
            {
                { "Apple", new Product("Apple", (decimal)0.75, (decimal)0.25) },
                { "Banana", new Product("Banana", (decimal)1.00, (decimal)0.00) },
                { "Orange", new Product("Orange", (decimal)1.25, (decimal)0.20) }
            };

            _dataAccessServiceMock.Setup(x => x.GetProductCatalog())
               .Returns(fakeProductCatalog);
        }
        

        [TestMethod]
        public void ScanVaildItemsTest()
        {
            //--Arrange
            string[] inputStringArray = new string[3] { "Apple", "Banana", "Orange" };

            List<Product> expected = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25),
                new Product("Banana", (decimal)1.00, (decimal)0.00),
                new Product("Orange", (decimal)1.25, (decimal)0.20)
            };

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ScanNoItemsTest()
        {
            //--Arrange
            string[] inputStringArray = new string[0];

            List<Product> expected = new List<Product>();

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ScanOneItemTest()
        {
            //--Arrange
            string[] inputStringArray = new string[1] { "Apple" };

            List<Product> expected = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25)
            };

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ScanRepeatItemsTest()
        {
            //--Arrange
            string[] inputStringArray = new string[2] { "Apple", "Apple" };

            List<Product> expected = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25),
                new Product("Apple", (decimal)0.75, (decimal)0.25)
            };

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ScanInvalidItemTest()
        {
            //--Arrange
            string checkoutItem = "InvalidItem";
            string[] inputStringArray = new string[1] { checkoutItem };

            List<Product> expected = new List<Product>();

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
            _kioskPrinterMock.Verify(x => x.PrintLineToConsole($"Unable to find item [{checkoutItem}] in product catalog."), Times.Once);

            _logMock.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>) It.IsAny<object>() ), 
                Times.Once);
        }

        [TestMethod]
        public void ScanInvalidItemsWithValidItemsTest()
        {
            //--Arrange
            string[] inputStringArray = new string[4] { "Apple", "InvalidItem1", "Banana", "InvalidItem2" };

            List<Product> expected = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25),
                new Product("Banana", (decimal)1.00, (decimal)0.00),
            };

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
            _kioskPrinterMock.Verify(x => x.PrintLineToConsole("Unable to find item [InvalidItem1] in product catalog."), Times.Once);
            _kioskPrinterMock.Verify(x => x.PrintLineToConsole("Unable to find item [InvalidItem2] in product catalog."), Times.Once);
            _logMock.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(2));
        }


        [TestMethod]
        public void ScanItemsWithExtraLinesTest()
        {
            //--Arrange
            string[] inputStringArray = new string[4] { "Apple", "", "Banana", "" };

            List<Product> expected = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25),
                new Product("Banana", (decimal)1.00, (decimal)0.00),
            };

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ScanItemsWithExtraWhiteSpaceTest()
        {
            //--Arrange
            string[] inputStringArray = new string[1] { "    Apple    "};

            List<Product> expected = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25),
            };

            //--Act
            List<Product> result = _itemScanner.ScanItems(inputStringArray);

            //--Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
