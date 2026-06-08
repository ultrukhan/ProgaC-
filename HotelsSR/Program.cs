using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using System.Xml.Linq;


namespace HotelSR {

    public class HotelLogic {
        public static XElement TaskA(IEnumerable<XElement> hotels, IEnumerable<XElement> guests, IEnumerable<XElement> categs, IEnumerable<XElement> brons, string city) {
            var data = (from h in hotels
                        where (string)h.Element("City") == city
                        join c in categs on (int)h.Element("Id") equals (int)c.Element("HotelId")
                        join b in brons on (int)c.Element("Id") equals (int)b.Element("CategId")
                        join g in guests on (int)b.Element("GuestId") equals (int)g.Element("Id")
                        select new
                        {
                            guest = (string)g.Element("LastName"),
                            hotel = (string)h.Element("Name"),
                            categ = (string)c.Element("Name")
                        });
            return new XElement("TaskA", new XAttribute("City", city),
                from d in data
                group d by d.guest into dg
                orderby dg.Key
                select new XElement("Guest", new XAttribute("LastName", dg.Key),
                    from dge in dg
                    group dge by dge.hotel into dgh
                    orderby dgh.Key
                    select new XElement("Hotel", new XAttribute("Name", dgh.Key),
                        from dghe in dgh
                        group dghe by dghe.categ into dghc
                        select new XElement("Category", new XAttribute("Name", dghc.Key))
                        )));
        }
        public static XElement TaskB(IEnumerable<XElement> hotels, IEnumerable<XElement> categs, IEnumerable<XElement> brons, DateTime start, DateTime end, double minRev)
        {
            var data = (from h in hotels
                        join c in categs on (int)h.Element("Id") equals (int)c.Element("HotelId")
                        join b in brons on (int)c.Element("Id") equals (int)b.Element("CategId")
                        where (DateTime)b.Element("ZDate") >= start && (DateTime)b.Element("ZDate") <= end
                        let ZDate = (DateTime)b.Element("ZDate")
                        let BDate = (DateTime)b.Element("BDate")
                        let BasePrice = (double)c.Element("BasePrice")
                        let days = (ZDate - BDate).TotalDays
                        let disc = days > 14
                        select new {
                            hotel = (string)h.Element("Name"),
                            Nights = (int)b.Element("Nights"),
                            Price = BasePrice * (int)b.Element("Nights"),
                            discount = disc
                        }
                        );
            return new XElement("TaskB", new XAttribute("Start", start), new XAttribute("End", end), new XAttribute("MinRev", minRev),
                from d in data
                group d by d.hotel into dg
                let rev = dg.Sum(x => x.discount ? x.Price * 0.8 : x.Price)
                where rev > minRev
                orderby rev descending
                select new XElement("Hotel", new XAttribute("Name", dg.Key), new XAttribute("NumOfNights", dg.Sum(x => x.Nights)), new XAttribute("Revenue", rev))
                );

        }
        public static XElement TaskC(IEnumerable<XElement> categs, IEnumerable<XElement> brons) {
            var data = (from c in categs
                        join b in brons on (int)c.Element("Id") equals (int)b.Element("CategId")
                        let BasePrice = (double)c.Element("BasePrice")
                        let ZDate = (DateTime)b.Element("ZDate")
                        let BDate = (DateTime)b.Element("BDate")
                        let days = (ZDate - BDate).TotalDays
                        let disc = days > 14
                        select new
                        {
                            categ = (string)c.Element("Name"),
                            Price = BasePrice * (int)b.Element("Nights"),
                            discount = disc
                        });
            return new XElement("TaskC",
                from d in data
                group d by d.categ into dc
                orderby dc.Key
                let rev = dc.Sum(x => x.discount ? x.Price * 0.8 : x.Price)
                select new XElement("Category", new XAttribute("Name", dc.Key), new XAttribute("Revenue", rev))
                );
        }
        public static XElement TaskD(IEnumerable<XElement> hotels, IEnumerable<XElement> guests, IEnumerable<XElement> categs, IEnumerable<XElement> brons)
        {
            var data = (from h in hotels
                        join c in categs on (int)h.Element("Id") equals (int)c.Element("HotelId")
                        join b in brons on (int)c.Element("Id") equals (int)b.Element("CategId")
                        join g in guests on (int)b.Element("GuestId") equals (int)g.Element("Id")
                        let BasePrice = (double)c.Element("BasePrice")
                        let ZDate = (DateTime)b.Element("ZDate")
                        let BDate = (DateTime)b.Element("BDate")
                        let days = (ZDate - BDate).TotalDays
                        let disc = days > 14
                        select new
                        {
                            guest = (string)g.Element("LastName"),
                            hotel = (string)h.Element("Name"),
                            Price = BasePrice * (int)b.Element("Nights"),
                            discount = disc
                        });
            return new XElement("TaskD",
                from d in data
                group d by d.hotel into dh
                orderby dh.Key
                let tempdata = (from dhe in dh
                                group dhe by dhe.guest into dhg
                                select new
                                {
                                    guest = dhg.Key,
                                    Spended = dhg.Sum(x => x.discount ? x.Price * 0.8 : x.Price)
                                })
                let max = tempdata.Max(x => x.Spended)
                select new XElement("Hotel", new XAttribute("Name", dh.Key), new XAttribute("MaxSpended", max),
                   from td in tempdata
                   where td.Spended == max
                   select new XElement("Guest", new XAttribute("LastName", td.guest), new XAttribute("Spended", td.Spended))
                   )

            );
        } 
    }
        
    public class Program{
        static void Main(string[] args) {
            var guests = XDocument.Load("guests.xml").Descendants("Guest");
            var hotels = XDocument.Load("hotels.xml").Descendants("Hotel");
            var categs = XDocument.Load("categs.xml").Descendants("Categ");
            var brons1 = XDocument.Load("brons1.xml").Descendants("Bron");
            var brons2 = XDocument.Load("brons2.xml").Descendants("Bron");
            var brons = brons1.Concat(brons2);
            var TaskARes = HotelLogic.TaskA(hotels, guests, categs, brons, "Lviv");
            var TaskBRes = HotelLogic.TaskB(hotels,categs, brons, new DateTime(2025,6,1), new DateTime(2026,7,20), 200);
            var TaskCRes = HotelLogic.TaskC(categs, brons);
            var TaskDRes = HotelLogic.TaskD(hotels, guests, categs, brons);
            TaskARes.Save("TaskA.xml");
            TaskBRes.Save("TaskB.xml");
            TaskCRes.Save("TaskC.xml");
            TaskDRes.Save("TaskD.xml");
        }
    }
}