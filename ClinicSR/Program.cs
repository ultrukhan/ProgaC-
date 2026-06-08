using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace ClinicSR {
    public class ClinicLogic {
        public static XElement TaskA(IEnumerable<XElement> patients, IEnumerable<XElement> doctors, IEnumerable<XElement> services, IEnumerable<XElement> visits, string spec) {
            var data = (from d in doctors
                        where (string)d.Element("Specialization") == spec
                        join v in visits on (int)d.Element("Id") equals (int)v.Element("DoctorId")
                        join s in services on (int)v.Element("ServiceId") equals (int)s.Element("Id")
                        join p in patients on (int)v.Element("PatientId") equals (int)p.Element("Id")
                        select new {
                            doctor = (string)d.Element("LastName"),
                            service = (string)s.Element("Name"),
                            patient = (string)p.Element("LastName")
                        });
            return new XElement("TaskA", new XAttribute("Spec", spec),
                from d in data
                group d by d.doctor into dd
                orderby dd.Key
                select new XElement("Doctor", new XAttribute("LastName", dd.Key),
                    from dde in dd
                    group dde by dde.service into dds
                    orderby dds.Key
                    select new XElement("Service", new XAttribute("Name", dds.Key),
                        from ddse in dds
                        group ddse by ddse.patient into ddsp
                        select new XElement("Patient", new XAttribute("LastName", ddsp.Key)))));
        }
        public static XElement TaskB(IEnumerable<XElement> services, IEnumerable<XElement> visits, DateTime start, DateTime end) {
            var data = (from v in visits
                        where (DateTime)v.Element("Date") >= start && (DateTime)v.Element("Date") <= end
                        join s in services on (int)v.Element("ServiceId") equals (int)s.Element("Id")
                        let weekday = ((DateTime)v.Element("Date")).DayOfWeek
                        let Finalprice = weekday == DayOfWeek.Saturday || weekday == DayOfWeek.Sunday ? (double)s.Element("BasePrice") * 0.75 : (double)s.Element("BasePrice")
                        select new
                        {
                            weekDay = weekday,
                            FinalPrice = Finalprice
                        });
            return new XElement("TaskB", new XAttribute("Start", start), new XAttribute("End", end),
                from d in data
                group d by d.weekDay into dw
                let rev = dw.Sum(x => x.FinalPrice)
                orderby rev descending
                select new XElement("WeekDay", new XAttribute("Title", dw.Key), new XAttribute("TotalVisit", dw.Count()), new XAttribute("Revenue", rev))
                );
        
        }
        public static XElement TaskC(IEnumerable<XElement> doctors, IEnumerable<XElement> services, IEnumerable<XElement> visits) {
            var data = (from d in doctors
                        join v in visits on (int)d.Element("Id") equals (int)v.Element("DoctorId")
                        join s in services on (int)v.Element("ServiceId") equals (int)s.Element("Id")
                        let weekday = ((DateTime)v.Element("Date")).DayOfWeek
                        let Finalprice = weekday == DayOfWeek.Saturday || weekday == DayOfWeek.Sunday ? (double)s.Element("BasePrice") * 0.75 : (double)s.Element("BasePrice")
                        select new
                        {
                            doctor = (string)d.Element("LastName"),
                            FInalPrice = Finalprice,
                            Minutes = (int)v.Element("Minutes")
                        });
            return new XElement("TaskC",
                from d in data
                group d by d.doctor into dd
                let minav = dd.Average(x => x.Minutes)
                where minav > 30
                let Rev = dd.Sum(x => x.FInalPrice)
                orderby Rev descending
                select new XElement("Doctor", new XAttribute("LastName", dd.Key), new XAttribute("Revenue", Rev)));

        }
        public static XElement TaskD(IEnumerable<XElement> doctors, IEnumerable<XElement> services, IEnumerable<XElement> visits) {
            var data = (from d in doctors
                        join v in visits on (int)d.Element("Id") equals (int)v.Element("DoctorId")
                        join s in services on (int)v.Element("ServiceId") equals (int)s.Element("Id")
                        let weekday = ((DateTime)v.Element("Date")).DayOfWeek
                        let Finalprice = weekday == DayOfWeek.Saturday || weekday == DayOfWeek.Sunday ? (double)s.Element("BasePrice") * 0.75 : (double)s.Element("BasePrice")
                        select new
                        {
                            doctor = (string)d.Element("LastName"),
                            FInalPrice = Finalprice,
                            service = (string)s.Element("Name")
                        });
            return new XElement("TaskD",
                from d in data
                group d by d.service into ds
                orderby ds.Key
                let tempdata = (from dse in ds
                                group dse by dse.doctor into dsd
                                select new
                                {
                                    doctor = dsd.Key,
                                    revenue = dsd.Sum(x => x.FInalPrice)
                                })
                let maxrev = tempdata.Max(x => x.revenue)
                select new XElement("Service", new XAttribute("Name", ds.Key), new XAttribute("MaxRev", maxrev),
                from td in tempdata
                where td.revenue == maxrev
                select new XElement("Doctor", new XAttribute("LastName", td.doctor), new XAttribute("Revenue", td.revenue)))


            );
        
        }
    }
    public class Program {
        static void Main(string[] args) {
            var doctors = XDocument.Load("doctors.xml").Descendants("Doctor");
            var patients = XDocument.Load("patients.xml").Descendants("Patient");
            var services = XDocument.Load("services.xml").Descendants("Service");
            var visits1 = XDocument.Load("visits1.xml").Descendants("Visit");
            var visits2 = XDocument.Load("visits2.xml").Descendants("Visit");
            var visits = visits1.Concat(visits2);
            var TaskARes = ClinicLogic.TaskA(patients, doctors, services, visits, "Surgeon");
            var TaskBRes = ClinicLogic.TaskB(services, visits, new DateTime(2026, 6, 1), new DateTime(2026, 6, 15));
            var TaskCRes = ClinicLogic.TaskC(doctors, services, visits);
            var TaskDRes = ClinicLogic.TaskD(doctors, services, visits);

            TaskARes.Save("TaskA.xml");
            TaskBRes.Save("TaskB.xml");
            TaskCRes.Save("TaskC.xml");
            TaskDRes.Save("TaskD.xml");
        }
    }
}