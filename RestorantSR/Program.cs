using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace SRRest {
    public class RestorantLogic {
        public static XElement TaskA(IEnumerable<XElement> restorants, IEnumerable<XElement> clients, IEnumerable<XElement> dishes, IEnumerable<XElement> deliveries, string city) {
            var data = (from d in deliveries
                        join ds in dishes on (int)d.Element("DishId") equals (int)ds.Element("Id")
                        join c in clients on (int)d.Element("ClientId") equals (int)c.Element("Id")
                        join r in restorants on (int)d.Element("RestorantId") equals (int)r.Element("Id")
                        where (string)r.Element("City") == city
                        select new
                        {
                            client = (string)c.Element("LastName"),
                            category = (string)ds.Element("Category"),
                            dish = (string)ds.Element("Name")
                        });
            return new XElement("TaskA", new XAttribute("City", city),
                from d in data
                group d by d.client into dc
                orderby dc.Key
                select new XElement("Client", new XAttribute("LastName", dc.Key),
                    from dce in dc
                    group dce by dce.category into dcc
                    orderby dcc.Key
                    select new XElement("Category", new XAttribute("CName", dcc.Key),
                        from dcce in dcc
                        group dcce by dcce.dish into dccd
                        select new XElement("Dish", new XAttribute("DName",dccd.Key))
                    )
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> dishes, IEnumerable<XElement> deliveries, DateTime start,DateTime end, int minRev) {
            var data = (from d in deliveries
                        where (DateTime)d.Element("Date") >= start && (DateTime)d.Element("Date") <= end
                        join ds in dishes on (int)d.Element("DishId") equals (int)ds.Element("Id")
                        select new {
                            category = (string)ds.Element("Category"),
                            price = (int)ds.Element("Price"),
                            numOf = (int)d.Element("NumOf"),
                        }
                        );
            return new XElement("TaskB", new XAttribute("Start", start), new XAttribute("End", end), new XAttribute("minRev", minRev),
                from d in data
                group d by d.category into dc
                let rev = dc.Sum(x => x.price * x.numOf)
                where rev >= minRev
                orderby rev descending
                select new XElement("Category", new XAttribute("Name", dc.Key), new XAttribute("DelivNum", dc.Count()), new XAttribute("Revenue", rev))
                );
        
        }
        public static XElement TaskC(IEnumerable<XElement> restorants, IEnumerable<XElement> dishes, IEnumerable<XElement> deliveries) {
            var data = (from d in deliveries
                        join ds in dishes on (int)d.Element("DishId") equals (int)ds.Element("Id")
                        join r in restorants on (int)d.Element("RestorantId") equals (int)r.Element("Id")
                        select new {
                            restorant = (string)r.Element("Name"),
                            price = (int)ds.Element("Price"),
                            numOf = (int)d.Element("NumOf"),
                        });
            return new XElement("TaskC",
                from d in data
                group d by d.restorant into dr
                orderby dr.Key
                let rev = dr.Sum(x => x.price * x.numOf)
                select new XElement("Restorant", new XAttribute("Name", dr.Key),new XAttribute("Revenue",rev))
                );
        }
        public static XElement TaskD(IEnumerable<XElement> clients, IEnumerable<XElement> dishes, IEnumerable<XElement> deliveries) {
            var data = (from d in deliveries
                        join ds in dishes on (int)d.Element("DishId") equals (int)ds.Element("Id")
                        join c in clients on (int)d.Element("ClientId") equals (int)c.Element("Id")
                        select new
                        {
                            dish = (string)ds.Element("Name"),
                            client = (string)c.Element("LastName"),
                            price = (int)ds.Element("Price"),
                            numOf = (int)d.Element("NumOf")
                        }
                        );
            return new XElement("TaskD",
                from d in data
                group d by d.dish into dd
                orderby dd.Key
                let tempdata = (from dde in dd
                                group dde by dde.client into ddc
                                select new
                                {
                                    spended = ddc.Sum(x => x.price * x.numOf),
                                    client= ddc.Key
                                })
                let maxSpended = tempdata.Max(x => x.spended)
                select new XElement("Dish", new XAttribute("Name", dd.Key), new XAttribute("MaxSpended", maxSpended),
                from td in tempdata
                where td.spended >= maxSpended
                select new XElement("Client", new XAttribute("LastName",td.client), new XAttribute("Spended", td.spended)))
                );
        }
    }
    class Program {
        static void Main(string[] args) {
            var clients = XDocument.Load("clients.xml").Descendants("Client");
            var dishes = XDocument.Load("dishes.xml").Descendants("Dish");
            var restorants = XDocument.Load("restorants.xml").Descendants("Restorant");
            var dev1 = XDocument.Load("delivs1.xml").Descendants("Delivery");
            var dev2 = XDocument.Load("delivs2.xml").Descendants("Delivery");
            var deliveries = dev1.Concat(dev2);
            var TaskAResult = RestorantLogic.TaskA(restorants, clients, dishes, deliveries,"Lviv");
            var TaskBResult = RestorantLogic.TaskB(dishes, deliveries, new DateTime(2026, 5, 10), new DateTime(2026, 5, 25), 500);
            var TaskCResult = RestorantLogic.TaskC(restorants, dishes, deliveries);
            var TaskDResult = RestorantLogic.TaskD(clients, dishes, deliveries);
            TaskAResult.Save("TaskA.xml");
            TaskBResult.Save("TaskB.xml");
            TaskCResult.Save("TaskC.xml");
            TaskDResult.Save("TaskD.xml");
        }
    }
}