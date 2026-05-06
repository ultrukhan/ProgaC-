using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace tasks {
    public class ShopLogic {
        public static double CalculateTotalByCategory(IEnumerable<XElement> orders, string categ) {
            return (from o in orders
                    where (string)o.Element("Category") == categ
                    select (double)o.Element("Amount")).Sum();
        
        }
        public static XElement GetFilterOrders(IEnumerable<XElement> orders, double minAmount) {
            return new XElement ("BigOrders",
                    from o in orders
                    where (double)o.Element("Amount") >= minAmount
                    select new XElement("Record",
                        new XElement("Category",(string)o.Element("Category")),
                        new XElement("Amount",(double)o.Element("Amount"))
                        )
                    );
        
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var doc = XDocument.Load("orders.xml");
            var OrdersTree = doc.Descendants("Order");

            double total = ShopLogic.CalculateTotalByCategory(OrdersTree, "Tech");
            Console.WriteLine($"Сума по Tech: {total}");

            XElement report = ShopLogic.GetFilterOrders(OrdersTree, 500);

            report.Save("report.xml");
        }
    }
}

