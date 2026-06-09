using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace Procat {

    public class ProcatLogic {
        public static XElement TaskA(IEnumerable<XElement> offices, IEnumerable<XElement> clients, IEnumerable<XElement> cars, IEnumerable<XElement> orends, string city) {
            var data = (from o in offices
                        where (string)o.Element("City") == city
                        join c in cars on (int)o.Element("Id") equals (int)c.Element("OfficeId")
                        join or in orends on (int)c.Element("Id") equals (int)or.Element("CarId")
                        join cl in clients on (int)or.Element("ClientId") equals (int)cl.Element("Id")
                        select new {
                            clas = (string)c.Element("Class"),
                            mark = (string)c.Element("Mark"),
                            client = (string)cl.Element("LastName")
                        });
            return new XElement("TaskA", new XAttribute("City", city),
                from d in data
                group d by d.clas into dc
                orderby dc.Key
                select new XElement("Class", new XAttribute("Name", dc.Key),
                    from dce in dc
                    group dce by dce.mark into dcm
                    orderby dcm.Key
                    select new XElement("Mark", new XAttribute("Name", dcm.Key),
                        from dcme in dcm
                        group dcme by dcme.client into dcmc
                        select new XElement("Client", new XAttribute("LastName",dcmc.Key)) 
                        )
                    )
                );
        
        }
        public static XElement TaskB(IEnumerable<XElement> offices, IEnumerable<XElement> cars, IEnumerable<XElement> orends, DateTime start, DateTime end, double minRev) {
            var data = (from o in offices
                        join c in cars on (int)o.Element("Id") equals (int)c.Element("OfficeId")
                        join or in orends on (int)c.Element("Id") equals (int)or.Element("CarId")
                        where (DateTime)or.Element("OrDate") >= start && (DateTime)or.Element("OrDate") <= end
                        select new {
                            office = (string)o.Element("Name"),
                            days = (int)or.Element("Days"),
                            Price = (int)or.Element("Days") * (double)c.Element("BasePrice")
                        }
                        );
            return new XElement("TaskB", new XAttribute("Start", start), new XAttribute("End", end), new XAttribute("MinRev", minRev),
                from d in data
                group d by d.office into dof
                let rev = dof.Sum(x => x.days > 7 ? x.Price * 0.85 : x.Price)
                where rev >= minRev
                orderby rev descending
                select new XElement("Office", new XAttribute("Name", dof.Key), new XAttribute("SumDays", dof.Sum(x => x.days)), new XAttribute("Revenue", rev))
                );
        }
        public static XElement TaskC(IEnumerable<XElement> cars, IEnumerable<XElement> orends) {
            var data = (from o in orends
                        join c in cars on (int)o.Element("CarId") equals (int)c.Element("Id")
                        select new
                        {
                            Price = (int)o.Element("Days") * (double)c.Element("BasePrice"),
                            clas = (string)c.Element("Class"),
                            days = (int)o.Element("Days")
                        });
            return new XElement("TaskC",
                from d in data
                group d by d.clas into dc
                orderby dc.Key
                let rev = dc.Sum(x => x.days > 7 ? x.Price * 0.85 : x.Price)
                select new XElement("Class", new XAttribute("Name", dc.Key), new XAttribute("Revenue", rev))
                );
        }
        public static XElement TaskD(IEnumerable<XElement> clients, IEnumerable<XElement> cars, IEnumerable<XElement> orends) {
            var data = (from o in orends
                        join c in cars on (int)o.Element("CarId") equals (int)c.Element("Id")
                        join cl in clients on (int)o.Element("ClientId") equals (int)cl.Element("Id")
                        select new
                        {
                            mark = (string)c.Element("Mark"),
                            client = (string)cl.Element("LastName"),
                            days = (int)o.Element("Days"),
                            Price = (int)o.Element("Days") * (double)c.Element("BasePrice")
                        });
            return new XElement("TaskD",
                from d in data
                group d by d.mark into dm
                orderby dm.Key
                let tempdata = (from dme in dm
                                group dme by dme.client into dmc
                                select new {
                                    client = dmc.Key,
                                    Spended = dmc.Sum(x => x.days > 7 ? x.Price * 0.85 : x.Price)
                                })
                let max = tempdata.Max(x => x.Spended)
                select new XElement("Mark", new XAttribute("Name",dm.Key), new XAttribute("MaxSpended",max),
                    from td in tempdata
                    where td.Spended == max
                    select new XElement("Client", new XAttribute("LastName",td.client), new XAttribute("Spended",td.Spended)))
            );
        }

    }
    public class Program {
        static void Main(string[] args) {
            var offices = XDocument.Load("offices.xml").Descendants("Office");
            var cars = XDocument.Load("cars.xml").Descendants("Car");
            var clients = XDocument.Load("clients.xml").Descendants("Client");
            var orends1 = XDocument.Load("orends1.xml").Descendants("Orend");
            var orends2 = XDocument.Load("orends2.xml").Descendants("Orend");
            var orends = orends1.Concat(orends2);
            var TaskARes = ProcatLogic.TaskA(offices, clients, cars, orends, "Lviv");
            var TaskBres = ProcatLogic.TaskB(offices, cars, orends, new DateTime(2026, 6, 1), new DateTime(2026, 6, 25), 500);
            var TaskCRes = ProcatLogic.TaskC(cars, orends);
            var TaskDRes = ProcatLogic.TaskD(clients, cars, orends);
            TaskARes.Save("TaskA.xml");
            TaskBres.Save("TaskB.xml");
            TaskCRes.Save("TaskC.xml");
            TaskDRes.Save("TaskD.xml");
        }
    }
}