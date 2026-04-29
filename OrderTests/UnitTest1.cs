using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using tasks; // Підключаємо твою програму!(namespace)

namespace ShopTests
{
    public class OrderTest
    {
        [Fact]
        public void GetTotalByCategory_correct()
        {

            var mockOrders = new List<XElement>
            {
            new XElement("Order", new XElement("Amount", 500), new XElement("Category", "Tech")),
            new XElement("Order", new XElement("Amount", 200), new XElement("Category", "Food")),
            new XElement("Order", new XElement("Amount", 1500), new XElement("Category", "Tech"))
            };

            double result = ShopLogic.CalculateTotalByCategory(mockOrders, "Tech");
            Assert.Equal(2000, result);
        }

        [Fact]
        public void GetFilterOrders_correct()
        {
            var mockOrders = new List<XElement>
            {
            new XElement("Order", new XElement("Amount", 500), new XElement("Category", "Tech")),
            new XElement("Order", new XElement("Amount", 1200), new XElement("Category", "Food"))
            };

            XElement result = ShopLogic.GetFilterOrders(mockOrders, 600);

            int count = result.Elements("Record").Count();
            Assert.Equal(1, count);

            string category = (string)result.Element("Record").Element("Category");
            Assert.Equal("Food", category);
        }
    }
}