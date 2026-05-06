using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SRCinema {
    public class CinemaLogic {
        public static XElement TaskA(IEnumerable<XElement> movies, IEnumerable<XElement> halls, IEnumerable<XElement> sessions, IEnumerable<XElement> tickets, IEnumerable<XElement> producers, IEnumerable<XElement> cinemas, string argcity) {
            var data = (from t in tickets
                        join s in sessions on (int)t.Element("SessionId") equals (int)s.Element("Id")
                        join m in movies on (int)s.Element("MovieId") equals (int)m.Element("Id")
                        join h in halls on (int)s.Element("HallId") equals (int)h.Element("Id")
                        join p in producers on (int)m.Element("ProducerId") equals (int)p.Element("Id")
                        join c in cinemas on (int)h.Element("CinemaId") equals (int)c.Element("Id")
                        where (string)c.Element("City") == argcity
                        orderby (string)c.Element("Name")
                        select new {
                            Cinema = (string)c.Element("Name"),
                            Producer = (string)p.Element("LastName"),
                            Movie = (string)m.Element("Name")
                        });
            return new XElement("CinemasReport", new XAttribute("City", argcity),
                    from d in data
                    group d by d.Cinema into dc
                    select new XElement("Cinema",
                    new XAttribute("CName", dc.Key),
                        from dce in dc
                        orderby dce.Producer
                        group dce by dce.Producer into dcp
                        select new XElement("Producer",
                            new XAttribute("PName", dcp.Key),
                            from dcpe in dcp
                            group dcpe by dcpe.Movie into dcpm
                            select new XElement("Movie",
                                new XAttribute("MName", dcpm.Key)
                            )
                        )
                    )
            );
        
        }
        public static XElement TaskB(IEnumerable<XElement> movies, IEnumerable<XElement> sessions, IEnumerable<XElement> tickets, DateTime start,DateTime end, int minRev) {
            return new XElement("RevenueByGenre", new XAttribute("Start", start), new XAttribute("End", end), new XAttribute("MinRev", minRev),
                from t in tickets
                join s in sessions on (int)t.Element("SessionId") equals (int)s.Element("Id")
                where (DateTime)s.Element("Date") >= start && (DateTime)s.Element("Date") <= end
                join m in movies on (int)s.Element("MovieId") equals (int)m.Element("Id")
                group t by (string)m.Element("Genre") into tg
                let rev = tg.Sum(x => (int)x.Element("Price"))
                where rev >= minRev
                select new XElement("Genre",
                    new XAttribute("Name", tg.Key),
                    new XAttribute("TicketCount", tg.Count()),
                    new XAttribute("Revenue", rev)
                )
            );
        }
    
    }
    class Program {
        static void Main(string[] args) {
            var movies = XDocument.Load("movies.xml").Descendants("Movie");
            var halls = XDocument.Load("halls.xml").Descendants("Hall");
            var producers = XDocument.Load("producers.xml").Descendants("Producer");
            var sessions = XDocument.Load("sessions.xml").Descendants("Session");
            var tickets = XDocument.Load("tickets.xml").Descendants("Ticket");
            var cinemas = XDocument.Load("cinemas.xml").Descendants("Cinema");
            var taskA = CinemaLogic.TaskA(movies, halls, sessions, tickets, producers, cinemas, "Kyiv");
            taskA.Save("TaskA.xml");
            var taskB = CinemaLogic.TaskB(movies, sessions, tickets, new DateTime(2026, 4, 29), new DateTime(2026, 5, 3), 500);
            taskB.Save("TaskB.xml");


        }
    }
}