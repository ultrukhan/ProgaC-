using System;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Collections.Generic;

namespace Pidgot_4
{
    public class FlLogic
    {
        public static XElement TaskA(IEnumerable<XElement> destinations, IEnumerable<XElement> flights, IEnumerable<XElement> tickets)
        {
            var data = from t in tickets
                       join f in flights on (int)t.Element("FlightId") equals (int)f.Element("Id")
                       join d in destinations on (int)f.Element("DestinationId") equals (int)d.Element("Id")
                       select new
                       {
                           price = (int)d.Element("PricePerFlight"),
                           reys = (int)f.Element("Id"),
                           punkt = (string)d.Element("Name")
                       };
            return new XElement("TaskA",
                from d in data
                group d by new
                {
                    d.reys,
                    d.punkt
                } into gg
                orderby gg.Key.punkt
                select new XElement("Reys",
                    new XAttribute("Id", gg.Key.reys),
                    new XAttribute("Sum", gg.Sum(x => x.price))
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> destinations, IEnumerable<XElement> passengers, IEnumerable<XElement> flights, IEnumerable<XElement> tickets)
        {
            var data = from t in tickets
                       join f in flights on (int)t.Element("FlightId") equals (int)f.Element("Id")
                       join d in destinations on (int)f.Element("DestinationId") equals (int)d.Element("Id")
                       join p in passengers on (int)t.Element("PassengerId") equals (int)p.Element("Id")
                       select new
                       {
                           pasSur = (string)p.Element("LastName"),
                           pasId = (int)p.Element("Id"),
                           punkt = (string)d.Element("Name"),
                           price = (int)d.Element("PricePerFlight"),
                           month = DateTime.Parse((string)t.Element("Date")).Month
                       };
            return new XElement("TaskB",
                from d in data
                group d by new
                {
                    d.pasId,
                    d.pasSur
                } into gg
                orderby gg.Key.pasSur
                let maxP = (from ng in gg
                            group ng by new { ng.punkt, ng.month } into fg
                            select new
                            {
                                punktPr = fg.Key,
                                paid = fg.Sum(x => x.price)
                            }
                            )
                let maximum = maxP.Max(x => x.paid)
                select new XElement("Passanger",
                    new XAttribute("Surname", gg.Key.pasSur),
                    from mg in maxP
                    where mg.paid == maximum
                    group mg by mg.punktPr into lg
                    select new XElement("Statistics",
                            new XAttribute("Punkt", lg.Key.punkt),
                            new XAttribute("Paid", maximum)
                    )
                )
            );
        }
        public static XElement TaskC(IEnumerable<XElement> destinations, IEnumerable<XElement> flights, IEnumerable<XElement> tickets)
        {
            var data = from t in tickets
                       join f in flights on (int)t.Element("FlightId") equals (int)f.Element("Id")
                       join d in destinations on (int)f.Element("DestinationId") equals (int)d.Element("Id")
                       select new
                       {
                           mark = (string)f.Element("AirplaneModel"),
                           punkt = (string)d.Element("Name"),
                           price = (int)d.Element("PricePerFlight")
                       };
            return new XElement("TaskC",
                from d in data
                group d by d.mark into gg
                orderby gg.Key
                select new XElement("Plane",
                    new XAttribute("Model", gg.Key),
                    from g in gg
                    group g by g.punkt into pgg
                    orderby pgg.Count() descending
                    select new XElement("Punkt",
                        new XAttribute("Name", pgg.Key),
                        new XAttribute("Total", pgg.Count())
                    )
                )
            );
        }
        public static XElement TaskD(IEnumerable<XElement> passengers, IEnumerable<XElement> tickets)
        {
            var data = from t in tickets
                       join p in passengers on (int)t.Element("PassengerId") equals (int)p.Element("Id")
                       select new
                       {
                           pasSur = (string)p.Element("LastName"),
                           pasId = (int)p.Element("Id"),
                           month = ((DateTime)t.Element("Date")).Month,
                           fl = (int)t.Element("FlightId")
                       };
            return new XElement("TaskD",
                from d in data
                group d by d.month into gg
                orderby gg.Key
                let bypas = (from g in gg
                             group g by new { g.pasId, g.pasSur } into ggg
                             select new
                             {
                                 sur = ggg.Key.pasSur,
                                 num = ggg.Count()
                             }
                            )
                let maxt = bypas.Max(x => x.num)
                select new XElement("Stat",
                    new XAttribute("Month", gg.Key),
                    from b in bypas
                    where b.num == maxt
                    orderby b.sur
                    select new XElement("PerPassanger",
                        new XAttribute("Surname", b.sur),
                        new XAttribute("Num", b.num)
                    )
                )
            );
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var destinations = XDocument.Load("Destinations.xml").Descendants("Destination");
            var passengers = XDocument.Load("Passengers.xml").Descendants("Passenger");
            var flights = XDocument.Load("Flights.xml").Descendants("Flight");
            var tic1 = XDocument.Load("Tickets_Part1.xml").Descendants("Ticket");
            var tic2 = XDocument.Load("Tickets_Part2.xml").Descendants("Ticket");
            var tickets = tic1.Concat(tic2);

            var taskA = FlLogic.TaskA(destinations, flights, tickets);
            taskA.Save("TaskA.xml");
            var taskB = FlLogic.TaskB(destinations, passengers, flights, tickets);
            taskB.Save("TaskB.xml");
            var taskC = FlLogic.TaskC(destinations, flights, tickets);
            taskC.Save("TaskC.xml");
            var taskD = FlLogic.TaskD(passengers, tickets);
            taskD.Save("TaskD.xml");
        }
    }
}