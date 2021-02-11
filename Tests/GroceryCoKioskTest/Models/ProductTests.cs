using GroceryCoKiosk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroceryCoKioskTest
{
    [TestClass]
    public class ProductTest
    {
        [TestMethod]
        public void ProductCreationTest()
        {
            //--Arrange
            string expectedName = "name";
            decimal expectedPrice = (decimal)1.50;
            decimal expectedDiscount = (decimal)0.50;


            //--Act
            Product actual = new Product(expectedName, expectedPrice, expectedDiscount);

            //--Assert
            Assert.AreEqual(expectedName, actual.Name);
            Assert.AreEqual(expectedPrice, actual.Price);
            Assert.AreEqual(expectedDiscount, actual.Discount);
        }
    }
}
