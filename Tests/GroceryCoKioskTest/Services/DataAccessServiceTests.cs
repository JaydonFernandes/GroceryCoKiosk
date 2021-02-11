using FluentAssertions;
using GroceryCoKiosk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GroceryCoKioskTest
{
    [TestClass]
    public class DataAccessServiceTests
    {
        private readonly DataAccessService _dataAccessService;
        private readonly Mock<ILogger<IDataAccessService>> _logMock = new Mock<ILogger<IDataAccessService>>();
        private readonly Mock<IConfiguration> _configMock = new Mock<IConfiguration>();

        public DataAccessServiceTests()
        {
            _dataAccessService = new DataAccessService(_logMock.Object, _configMock.Object);
        }

        private readonly static List<Product> sampleValidProducts = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25),
                new Product("Banana", (decimal)1.00, (decimal)0.00),
                new Product("Orange", (decimal)1.25, (decimal)0.20)};

        private readonly static List<Product> sampleInvalidProducts = new List<Product>() {
                new Product("Apple", (decimal)0.75, (decimal)0.25),
                new Product(null, 0, 0),
                new Product("Banana", (decimal)1.00, (decimal)0.00),
                new Product("Invalid", (decimal)-1.25, (decimal)-0.20),
                new Product("Orange", (decimal)1.25, (decimal)0.20)};

        private readonly string _JSONValidSampleCatalog = JsonConvert.SerializeObject(sampleValidProducts);
        private readonly string _JSONInvalidSampleCatalog = JsonConvert.SerializeObject(sampleInvalidProducts);

        private readonly string _testCatalogFileName = "testCatalog.json";

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            if (TestContext.TestName == "ValidCatalogFileLocationTest")
            {
                var path = Directory.GetCurrentDirectory();
                if (!string.IsNullOrEmpty(_testCatalogFileName))
                {
                    File.AppendAllText(_testCatalogFileName, _JSONValidSampleCatalog);
                }
            }

            if (TestContext.TestName == "CatalogFileWithInvalidDataTest")
            {
                var path = Directory.GetCurrentDirectory();
                if (!string.IsNullOrEmpty(_testCatalogFileName))
                {
                    File.AppendAllText(_testCatalogFileName, _JSONInvalidSampleCatalog);
                }
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (TestContext.TestName == "ValidCatalogFileLocationTest")
            {
                if (!string.IsNullOrEmpty(_testCatalogFileName))
                {
                    File.Delete(_testCatalogFileName);
                }
            }

            if (TestContext.TestName == "CatalogFileWithInvalidDataTest")
            {
                if (!string.IsNullOrEmpty(_testCatalogFileName))
                {
                    File.Delete(_testCatalogFileName);
                }
            }
        }


        [TestMethod]
        public void ValidCatalogFileLocationTest()
        {

            //--Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"DataLocation", _testCatalogFileName}
            };

            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            DataAccessService dataAccessService = new DataAccessService(_logMock.Object, testConfiguration);

            Hashtable expected = new Hashtable();
            foreach (Product product in sampleValidProducts)
            {
                expected.Add(product.Name, (Product)product);
            }

            //--Act
            Hashtable result = dataAccessService.GetProductCatalog();

            //--Assert
            Assert.AreEqual(expected.Count, result.Count);
            foreach (DictionaryEntry expectedKV in expected)
            {
                result[expectedKV.Key].Should().BeEquivalentTo(expected[expectedKV.Key]);
            }
        }

        [TestMethod]
        public void CatalogFileWithInvalidDataTest()
        {

            //--Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"DataLocation", _testCatalogFileName}
            };

            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            DataAccessService dataAccessService = new DataAccessService(_logMock.Object, testConfiguration);

            Hashtable expected = new Hashtable();
            foreach (Product product in sampleValidProducts)
            {
                expected.Add(product.Name, (Product)product);
            }

            //--Act
            Hashtable result = dataAccessService.GetProductCatalog();

            //--Assert
            Assert.AreEqual(expected.Count, result.Count);
            foreach (DictionaryEntry expectedKV in expected)
            {
                result[expectedKV.Key].Should().BeEquivalentTo(expected[expectedKV.Key]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void InvalidCatalogFileLocationTest()
        {
            //--Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"DataLocation", "InvalidLocation"}
            };

            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            DataAccessService dataAccessService = new DataAccessService(_logMock.Object, testConfiguration);

            //--Act
            Hashtable result = dataAccessService.GetProductCatalog();

            //--Assert
            // Should throw FileNotFoundException
            _logMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(1));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyCatalogFileLocationTest()
        {
            //--Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"DataLocation", ""}
            };

            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            DataAccessService dataAccessService = new DataAccessService(_logMock.Object, testConfiguration);

            //--Act
            Hashtable result = dataAccessService.GetProductCatalog();

            //--Assert
            // Should throw ArgumentException
            _logMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullCatalogFileLocationTest()
        {
            //--Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"DataLocation", null}
            };

            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            DataAccessService dataAccessService = new DataAccessService(_logMock.Object, testConfiguration);

            //--Act
            Hashtable result = dataAccessService.GetProductCatalog();

            //--Assert
            // Should throw ArgumentNullException
            _logMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NoCatalogFileLocationTest()
        {
            //--Arrange
            var inMemorySettings = new Dictionary<string, string> {};

            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            DataAccessService dataAccessService = new DataAccessService(_logMock.Object, testConfiguration);

            //--Act
            Hashtable result = dataAccessService.GetProductCatalog();

            //--Assert
            // Should throw ArgumentNullException
            _logMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(1));
        }
    }
}
