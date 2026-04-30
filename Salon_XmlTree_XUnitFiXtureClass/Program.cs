using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Linq;

namespace SalonTask
{
    public class Meneger { 
        public int Id { get; set; }
        private string _LastName;
        public string LastName {
            get { return _LastName; }
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("LastName cant be null");
                }
                _LastName = value;
            }
        }
        private int _Stage;
        public int Stage {
            get { return _Stage; }
            set {
                if (value < 0)
                    throw new ArgumentException("Stage cant be negative");
                _Stage = value;
            }
        }
        public Meneger(int id,string ln,int st) {
            Id = id;
            _LastName = ln;
            _Stage = st;
        }
    }
    public static class SalonLogic
    {
        public static XElement CreateMonthReport(IEnumerable<XElement> menegers, IEnumerable<XElement> brands, IEnumerable<XElement> cars, IEnumerable<XElement> buyments, int month)
        {
            var data = (from b in buyments
                        where (int)b.Element("Month") == month
                        join m in menegers on (int)b.Element("MenegerId") equals (int)m.Element("Id")
                        join c in cars on (int)b.Element("CarId") equals (int)c.Element("Id")
                        join br in brands on (int)c.Element("BrandId") equals (int)br.Element("Id")
                        orderby (int)m.Element("Stage") descending
                        select new
                        {
                            Meneger = (string)m.Element("LastName"),
                            Brand = (string)br.Element("Name"),
                            Model = (string)c.Element("Model"),
                        });
            return new XElement("MonthReport", new XAttribute("Month", month),
                from d in data
                group d by d.Meneger into dm
                select new XElement("Meneger",
                    new XAttribute("LastName", dm.Key),
                    from dme in dm
                    group dme by dme.Brand into dmb
                    select new XElement("Brand",
                        new XAttribute("Name", dmb.Key),
                        from dmbe in dmb
                        select new XElement("Car",
                        new XAttribute("Model", dmbe.Model)
                        )
                    )
                )
            );
        }
        public static XElement CreateCountryStatistic(IEnumerable<XElement> brands, IEnumerable<XElement> cars, IEnumerable<XElement> buyments, int minRev)
        {
            return new XElement("CountryStatistic",new XAttribute("MinRev",minRev),
                from b in buyments
                join c in cars on (int)b.Element("CarId") equals (int)c.Element("Id")
                join br in brands on (int)c.Element("BrandId") equals (int)br.Element("Id")
                group (int)b.Element("Price") by (string)br.Element("Country") into g
                let total = g.Sum()
                where total > minRev
                orderby total descending
                select new XElement("Country",
                    new XAttribute("Name", g.Key),
                    new XAttribute("Count", g.Count()),
                    new XAttribute("Revenue", g.Sum())
                    )
                );

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var menegers = XDocument.Load("menegers.xml").Descendants("Meneger");
            var brands = XDocument.Load("brands.xml").Descendants("Brand");
            var cars = XDocument.Load("cars.xml").Descendants("Car");
            var buyments = XDocument.Load("buyments.xml").Descendants("Buyment");
            var result1 = SalonLogic.CreateMonthReport(menegers, brands, cars, buyments, 5);
            result1.Save("MonthReport.xml");
            var result2 = SalonLogic.CreateCountryStatistic(brands, cars, buyments, 10000);
            result2.Save("CountryStatistics.xml");

        }
    }
}