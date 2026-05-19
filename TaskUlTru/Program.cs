using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public static List<Product> LoadProducts(string path)
		{
			if (!File.Exists(path)) return new List<Product>();
			var xdoc = XDocument.Load(path);
			return xdoc.Descendants("Product").Select(p => new Product
			{
				Id = (int)p.Element("Id"),
				Name = (string)p.Element("Name"),
				Category = (string)p.Element("Category"),
				Price = (double)p.Element("Price")
			}).ToList();
		}

		public static List<Customer> LoadCustomers(string path) 
		{
			if (!File.Exist(path)) return new List<Customer>();
			var xdoc = XDocument.Load(path);
			return xdoc.Descendants("Customer").Select(c => new Customer
			{
				Id = (int)c.Element("Id"),
				FullName = (string)c.Element("Name"),
				City = (string)c.Element("City")
			}).ToList();
		}

		public static List<Order> LoadOrders(string path)
		{
			if (!File.Exist(path)) return new List<Order>;
			var xdoc = XDocument.Load(path);
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
			var XMLReport = new XElement("TaskA",
				from o in orders
				join p in products on o.ProductId equals p.Id
				group o by p.Category into g
				let count = g.Count()
				orderby count descending
				select new XElement("CategoryStat",
					new XElement("Category", g.Key),
					new XElement("Count", count))
			);

			XMLReport.Save(outpath);
			Console.WriteLine($"Task A saved to {outpath}");
		}

		public static void TaskB(List<Customer> customers, List<Order> orders, List<Product> products, 
			string outpath, DateTime startdate, DateTime enddate)
		{
			var XMLReport = new XElement("TaskB",
				from o in orders
				join c in customers on o.CustomerId equals c.Id
				join p in products on o.ProductId equals p.Id
				where o.OrderDate >= startdate && o.OrderDate <= enddate
				group o by c.Id into g
				let sum = g.Sum()
				select nem XElement("TaskBStatistics",
					new XElement("Customer", g.Key),
					new XElement("Sum", sum)
				)
			);

			XMLReport.Save(outpath);
			Console.WriteLine($"Task B saved to {outpath}"
		}

		public static void GenerateDataXML()
		{

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
