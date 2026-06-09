using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;


namespace Pidgot_1
{
    public class AutLogic
    {
        public static XElement TaskA(IEnumerable<XElement> datas, IEnumerable<XElement> ofises, IEnumerable<XElement> clients, IEnumerable<XElement> avtos, string city)
        {
            var data = from d in datas
                       join a in avtos on (int)d.Element("A_id") equals (int)a.Element("A_id")
                       join o in ofises on (int)a.Element("O_id") equals (int)o.Element("O_id")
                       where (string)o.Element("City") == city
                       join c in clients on (int)d.Element("C_id") equals (int)c.Element("C_id")
                       select new
                       {
                           clas = (string)a.Element("Class"),
                           marka = (string)a.Element("Marka"),
                           sur = (string)c.Element("Sur")
                       };
            return new XElement("TaskA",
                from d in data
                group d by d.clas into gc
                orderby gc.Key
                select new XElement("Car",
                    new XAttribute("Class", gc.Key),
                    from g in gc
                    group g by g.marka into mg
                    orderby mg.Key
                    select new XElement("Details",
                        new XAttribute("Marka", mg.Key),
                        from m in mg
                        group m by m.sur into fg
                        select new XElement("Client",
                            new XAttribute("Surname", fg.Key)
                        )
                    )
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> datas, IEnumerable<XElement> ofises, IEnumerable<XElement> avtos, DateTime start, DateTime end, double minDoh)
        {
            var data = from d in datas
                       where (DateTime)d.Element("StartDate") >= start && (DateTime)d.Element("StartDate") <= end
                       join a in avtos on (int)d.Element("A_id") equals (int)a.Element("A_id")
                       join o in ofises on (int)a.Element("O_id") equals (int)o.Element("O_id")
                       let price = (int)d.Element("Days") * (int)a.Element("BasePrice")
                       let paid = (int)d.Element("Days") > 7 ? price * 0.85 : price
                       select new
                       {
                           ofis = (string)o.Element("Name"),
                           days = (int)d.Element("Days"),
                           doh = paid
                       };
            return new XElement("TaskB",
                from d in data
                group d by d.ofis into gg
                where gg.Sum(x => x.doh) >= minDoh
                orderby gg.Sum(x => x.doh) descending
                select new XElement("Ofise",
                    new XAttribute("Name", gg.Key),
                    new XAttribute("Days", gg.Sum(x => x.days)),
                    new XAttribute("Dohid", gg.Sum(x => x.doh))
                )
            );
        }
        public static XElement TaskC(IEnumerable<XElement> datas, IEnumerable<XElement> avtos)
        {
            var data = from d in datas
                       join a in avtos on (int)d.Element("A_id") equals (int)a.Element("A_id")
                       let price = (int)d.Element("Days") * (int)a.Element("BasePrice")
                       let paid = (int)d.Element("Days") > 7 ? price * 0.85 : price
                       select new
                       {
                           clas = (string)a.Element("Class"),
                           doh = paid
                       };
            return new XElement("TaskC",
                from d in data
                group d by d.clas into g
                orderby g.Key
                select new XElement("Statistics",
                    new XAttribute("Class", g.Key),
                    new XAttribute("TotalPaid", g.Sum(x => x.doh))
                )
            );
        }
        public static XElement TaskD(IEnumerable<XElement> datas, IEnumerable<XElement> clients, IEnumerable<XElement> avtos)
        {
            var data = from d in datas
                       join c in clients on (int)d.Element("C_id") equals (int)c.Element("C_id")
                       join a in avtos on (int)d.Element("A_id") equals (int)a.Element("A_id")
                       let price = (int)d.Element("Days") * (int)a.Element("BasePrice")
                       let paid = (int)d.Element("Days") > 7 ? price * 0.85 : price
                       select new
                       {
                           marka = (string)a.Element("Marka"),
                           doh = paid,
                           client = (string)c.Element("Sur")
                       };
            return new XElement("TaskD",
                from d in data
                group d by d.marka into gg
                orderby gg.Key
                let byClie = from g in gg
                             group g by g.client into ng
                             select new
                             {
                                 surname = ng.Key,
                                 total = ng.Sum(x => x.doh)
                             }
                let maxDoh = byClie.Max(x => x.total)
                select new XElement("Statistics",
                    new XAttribute("Marka", gg.Key),
                    from gn in gg
                    group gn by gn.client into fg
                    where fg.Sum(x => x.doh) == maxDoh
                    select new XElement("Clients",
                        new XAttribute("Paid", fg.Sum(x => x.doh)),
                        new XAttribute("Surname", fg.Key)
                    )
                )
            );
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var ofises = XDocument.Load("ofises.xml").Descendants("Ofis");
            var clients = XDocument.Load("clients.xml").Descendants("Client");
            var avtos = XDocument.Load("avtos.xml").Descendants("Avto");
            var datas1 = XDocument.Load("datas1.xml").Descendants("Data");
            var datas2 = XDocument.Load("datas2.xml").Descendants("Data");
            var datas = datas1.Concat(datas2);

            var taskA = AutLogic.TaskA(datas, ofises, clients, avtos, "Lviv");
            taskA.Save("TaskA.xml");
            var taskB = AutLogic.TaskB(datas, ofises, avtos, new DateTime(2026, 01, 01), new DateTime(2026, 06, 06), 13000);
            taskB.Save("TaskB.xml");
            var taskС = AutLogic.TaskC(datas, avtos);
            taskС.Save("TaskС.xml");
            var taskD = AutLogic.TaskD(datas, clients, avtos);
            taskD.Save("TaskD.xml");
        }
    }
}