using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FitnessSr
{

    public class FitnessLogic
    {
        public static XElement TaskA(IEnumerable<XElement> clubs, IEnumerable<XElement> clients, IEnumerable<XElement> coaches, IEnumerable<XElement> workouts, string city)
        {
            var data = (from c in clubs
                        where (string)c.Element("City") == city
                        join w in workouts on (int)c.Element("Id") equals (int)w.Element("ClubId")
                        join cl in clients on (int)w.Element("ClientId") equals (int)cl.Element("Id")
                        join co in coaches on (int)w.Element("CoachId") equals (int)co.Element("Id")
                        select new
                        {
                            club = (string)c.Element("Name"),
                            coachrole = (string)co.Element("Role"),
                            coach = (string)co.Element("LastName")
                        });
            return new XElement("CityClubReport", new XAttribute("City", city),
                from d in data
                orderby d.club
                group d by d.club into dc
                select new XElement("Club",
                    new XAttribute("Name", dc.Key),
                    from dce in dc
                    orderby dce.coachrole
                    group dce by dce.coachrole into dcc
                    select new XElement("CoachRole",
                    new XAttribute("Role", dcc.Key),
                        from cl in dcc
                        group cl by cl.coach into clc
                        select new XElement("Coach",
                        new XAttribute("LastName", clc.Key)
                        )
                    )
                )

            );

        }
        public static XElement TaskB(IEnumerable<XElement> coaches, IEnumerable<XElement> workouts,
            DateTime start, DateTime end, int minRev)
        {

            var data = (from w in workouts
                        where (DateTime)w.Element("Date") >= start && (DateTime)w.Element("Date") <= end
                        join c in coaches on (int)w.Element("CoachId") equals (int)c.Element("Id")
                        select new
                        {
                            coachrole = (string)c.Element("Role"),
                            price = (int)w.Element("Price")
                        });
            return new XElement("RoleRevenueReport", new XAttribute("StartDate", start), new XAttribute("EndDate", end), new XAttribute("MinRev", minRev),
                from d in data
                group d by d.coachrole into dr
                let rev = dr.Sum(x => x.price)
                orderby rev descending
                where rev >= minRev
                select new XElement("CoachRole",
                    new XAttribute("Role", dr.Key),
                    new XAttribute("Workouts", dr.Count()),
                    new XAttribute("Revenue", rev)
                )

            );
        }
        public static XElement TaskC(IEnumerable<XElement> coaches, IEnumerable<XElement> workouts)
        {
            return new XElement("CoachRevenueReport",
                from w in workouts
                join c in coaches on (int)w.Element("CoachId") equals (int)c.Element("Id")
                group w by (string)c.Element("LastName") into wc
                orderby wc.Key
                let rev = wc.Sum(x => (int)x.Element("Price"))
                select new XElement("Coach",
                    new XAttribute("LastName", wc.Key),
                    new XAttribute("Revenue", rev)
                )
            );
        }
        public static XElement TaskD(IEnumerable<XElement> clients, IEnumerable<XElement> coaches, IEnumerable<XElement> workouts)
        {
            var data = (from w in workouts
                        join c in coaches on (int)w.Element("CoachId") equals (int)c.Element("Id")
                        join cl in clients on (int)w.Element("ClientId") equals (int)cl.Element("Id")
                        select new
                        {
                            client = (string)cl.Element("LastName"),
                            coach = (string)c.Element("LastName"),
                            price = (int)w.Element("Price"),
                        });
            return new XElement("ClientFavCoaches",
                from d in data
                group d by d.client into dc
                orderby dc.Key
                let clientdata = (from c in dc
                                  group c by c.coach into cc
                                  select new
                                  {
                                      coach = cc.Key,
                                      spended = cc.Sum(x => x.price),
                                      workouts = cc
                                  })
                let maxspended = clientdata.Max(x => x.spended)
                select new XElement("Client", new XAttribute("LastName", dc.Key), new XAttribute("MaxSpended", maxspended),
                from cd in clientdata
                where cd.spended == maxspended
                select new XElement("Coach", new XAttribute("LastName", cd.coach))
                )


            );

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var clubs = XDocument.Load("clubs.xml").Descendants("Club");
            var clients = XDocument.Load("clients.xml").Descendants("Client");
            var coaches = XDocument.Load("coaches.xml").Descendants("Coach");
            var workouts = XDocument.Load("workouts.xml").Descendants("Workout");
            var AREsult = FitnessLogic.TaskA(clubs, clients, coaches, workouts, "Lviv");
            var BREsult = FitnessLogic.TaskB(coaches, workouts, new DateTime(2026, 5, 1), new DateTime(2026, 5, 3), 100);
            var CResult = FitnessLogic.TaskC(coaches, workouts);
            var DResult = FitnessLogic.TaskD(clients, coaches, workouts);
            AREsult.Save("TaskAResult.xml");
            BREsult.Save("TaskBResult.xml");
            CResult.Save("TaskCResult.xml");
            DResult.Save("TaskDResult.xml");


        }
    }
}