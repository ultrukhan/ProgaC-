using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;

namespace StoreTask
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
    }

    public class Order
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
    }
    class Features {
        public static List<Product> LoadProducts(string filepath) {
            if (!File.Exists(filepath)) return new List<Product>();
            var xdoc = XDocument.Load(filepath);
            return xdoc.Descendants("Product").Select(p => new Product
            {
                Id = (int)p.Element("Id"),
                Name = (string)p.Element("Name"),
                Category = (string)p.Element("Category"),
                Price = (double)p.Element("Price")
            }).ToList();
        }
        public static List<Customer> LoadCustomers(string filepath) {
            if (!File.Exists(filepath)) return new List<Customer>();
            var xdoc = XDocument.Load(filepath);
            return xdoc.Descendants("Customer").Select(c => new Customer
            {
                Id = (int)c.Element("Id"),
                FullName = (string)c.Element("FullName"),
                City = (string)c.Element("City")
            }).ToList();
        
        }
        public static List<Order> LoadOrders(string filepath) {
            if (!File.Exists(filepath)) return new List<Order>();
            var xdoc = XDocument.Load(filepath);
            return xdoc.Descendants("Order").Select(o => new Order
            {
                CustomerId = (int)o.Element("CustomerId"),
                ProductId = (int)o.Element("ProductId"),
                OrderDate = (DateTime)o.Element("OrderDate"),
                Quantity = (int)o.Element("Quantity")
            }).ToList();
        }
        public static void TaskA(List<Product> products, List<Order> orders, string outpath)
        {
            var result = new XElement("OrderCounts",
                from p in products
                join o in orders on p.Id equals o.ProductId
                group o by p.Category into g
                let count = g.Count()
                orderby count descending
                select new XElement("CategoryStat",
                new XElement("Category", g.Key),
                new XElement("Count", count))
                );
            result.Save(outpath);
            Console.WriteLine($"TaskA completed. Output saved to {outpath}");
        }
        public static double TaskB(List<Order> orders, List<Customer> customers, List<Product> products, int cid, DateTime start, DateTime end) {
            var result = (from order in orders
                          join customer in customers on order.CustomerId equals customer.Id
                          join product in products on order.ProductId equals product.Id
                          where customer.Id == cid && order.OrderDate >= start && order.OrderDate <= end
                          let spended = product.Price * order.Quantity
                          select spended).Sum();
            return result;        
        }
        public static void TaskC(List<Order> orders, List<Customer> customers, List<Product> products, string outpath) {
            var result = new XElement("CountryClientsTovars",
                from o in orders
                join c in customers on o.CustomerId equals c.Id
                group o by c.City into gc
                select new XElement("CityStat",
                    new XAttribute("City", gc.Key),
                    from g in gc
                    join c in customers on g.CustomerId equals c.Id
                    group g by c.FullName into gf
                    select new XElement("Customers",
                        new XAttribute("FullName", gf.Key),
                            from gfe in gf
                            join p in products on gfe.ProductId equals p.Id
                            select new XElement("Purchase",
                            new XAttribute("Product", p.Name))
                            )
                    )
                );
            result.Save(outpath);
        
        }

    }
    class Program
    {

        static void Main(string[] args)
        {


            Console.WriteLine("Роботу завершено! Перевіряй XML-файли.");
        }

    }
}