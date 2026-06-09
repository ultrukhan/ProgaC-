using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SR
{
    public class ObservLogic
    {
        public static XElement TaskA(IEnumerable<XElement> cObjs, IEnumerable<XElement> astronoms, IEnumerable<XElement> seanss)
        {
            return new XElement("TaskA",
                from s in seanss
                join a in astronoms on (int)s.Element("AstrId") equals (int)a.Element("AstrId")
                join c in cObjs on (int)s.Element("CObjId") equals (int)c.Element("CObjId")
                group new
                {
                    astronom = (string)a.Element("Surname"),
                    hours = (int)s.Element("Truv")
                } by (string)c.Element("Name") into g
                orderby g.Key
                select new XElement("ByObject",
                    new XAttribute("Object", g.Key),
                    from gg in g
                    group gg by gg.astronom into ng
                    orderby g.Key
                    select new XElement("Stats",
                        new XAttribute("Astronom", ng.Key),
                        new XAttribute("Time", ng.Sum(x => x.hours))
                        )
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> cObjs, IEnumerable<XElement> astronoms, IEnumerable<XElement> seanss, IEnumerable<XElement> rTels)
        {
            var data = from s in seanss
                       join a in astronoms on (int)s.Element("AstrId") equals (int)a.Element("AstrId")
                       join c in cObjs on (int)s.Element("CObjId") equals (int)c.Element("CObjId")
                       join r in rTels on (int)s.Element("RTelId") equals (int)r.Element("RTelId")
                       let summa = (int)s.Element("Truv") * (int)c.Element("Price")
                       group new
                       {
                           summa,
                           month = ((DateTime)s.Element("Date")).Month,
                           telescope = (string)r.Element("Title")
                       } by (string)a.Element("Surname") into g
                       orderby g.Key
                       select g;
            return new XElement("TaskB",
                from d in data
                let monthlyStats = from dd in d
                                   group dd by new { dd.month, dd.telescope } into mg
                                   select new
                                   {
                                       Month = mg.Key.month,
                                       Telescope = mg.Key.telescope,
                                       TotalSum = mg.Sum(x => x.summa)
                                   }
                let maxSum = monthlyStats.Max(x => x.TotalSum)
                select new XElement("Astronom",
                    new XAttribute("Name", d.Key),
                    from ms in monthlyStats
                    where ms.TotalSum == maxSum
                    orderby ms.Month, ms.Telescope
                    select new XElement("Teleskope",
                        new XAttribute("Month", ms.Month),
                        new XAttribute("Name", ms.Telescope),
                        new XAttribute("Spent", ms.TotalSum)
                    )
                )
            );
        }
        class Program
        {
            static void Main(string[] args)
            {
                var astronoms = XDocument.Load("astronoms.xml").Descendants("Astronom");
                var cObjs = XDocument.Load("cObjs.xml").Descendants("CObj");
                var rTels = XDocument.Load("rTels.xml").Descendants("RTel");
                var seanss = XDocument.Load("seanss.xml").Descendants("Seans");

                var taskA = ObservLogic.TaskA(cObjs, astronoms, seanss);
                taskA.Save("TaskA.xml");
                var taskB = ObservLogic.TaskB(cObjs, astronoms, seanss, rTels);
                taskB.Save("TaskB.xml");

            }
        }
    }
}