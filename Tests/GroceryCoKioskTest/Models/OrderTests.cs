using GroceryCoKiosk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GroceryCoKioskTest
{
    [TestClass]
    public class OrderTest
    {
        [TestMethod]
        public void OrderWithOneItemTest()
        {
            //--Arrange
            List<Product> products = new List<Product>()
            {
                new Product("ItemName", 2, 1)
            };

            //--Act
            Order actual= new Order(products);

            //--Assert
            Assert.AreEqual(2, actual.SubTotal);
            Assert.AreEqual(1, actual.TotalDiscount);
            Assert.AreEqual(1, actual.Products.Count);
        }

        [TestMethod]
        public void OrderWithManyItemTest()
        {
            //--Arrange
            List<Product> products = new List<Product>()
            {
                new Product("ItemName1", 2, 1),
                new Product("ItemName2", 2, 1)
            };

            //--Act
            Order actual = new Order(products);

            //--Assert
            Assert.AreEqual(4, actual.SubTotal);
            Assert.AreEqual(2, actual.TotalDiscount);
            Assert.AreEqual(2, actual.Products.Count);
        }
    }
}
