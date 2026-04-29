using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

// подати без повторень = додаткове групування 
namespace MedProg {
    using System;

    namespace MedProg
    {
        public class Patient
        {
            // Звичайна авто-властивість
            public int Id { get; set; }

            private string _lastName;
            public string LastName
            {
                get { return _lastName; }
                set
                {
                    // Якщо прізвище пусте — кидаємо помилку
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException("Прізвище не може бути порожнім!");
                    _lastName = value;
                }
            }

            private int _age;
            public int Age
            {
                get { return _age; }
                set
                {
                    // Якщо вік від'ємний — кидаємо помилку
                    if (value < 0)
                        throw new ArgumentException("Вік не може бути менше нуля!");
                    _age = value;
                }
            }
        }
    }
    public static class MedLogic {
        public static XElement CreateReceptionReport(IEnumerable<XElement> doctors, IEnumerable<XElement> patients, IEnumerable<XElement> departments, IEnumerable<XElement> receptions, int year) {
            var Data = (from r in receptions
                        where (int)r.Element("Year") == year
                        join p in patients on (int)r.Element("PatientId") equals (int)p.Element("Id")
                        join d in doctors on (int)r.Element("DoctorId") equals (int)d.Element("Id")
                        join dep in departments on (int)r.Element("DepartmentId") equals (int)dep.Element("Id")
                        select new
                        {
                            Departement = (string)dep.Element("Name"),
                            Doctor = (string)d.Element("LastName"),
                            Patient = (string)p.Element("LastName"),
                        }
                        );
            return new XElement("YearReceptionReport", new XAttribute("Year", year),
                from d in Data
                orderby d.Departement descending
                group d by d.Departement into g
                select new XElement("Department",
                    new XAttribute("Name", g.Key),
                    from gd in g
                    orderby gd.Patient
                    select new XElement("DoctorPatient",
                        new XAttribute("Doctor", gd.Doctor),
                        new XAttribute("Patient", gd.Patient)
                        )
                    )
                );
        }
        public static XElement PatientReport(IEnumerable<XElement> patients, IEnumerable<XElement> doctors, IEnumerable<XElement> receptions,string keyword) {
            return new XElement("PatientReport", new XAttribute("Keyword", keyword),
                from p in patients
                where ((string)p.Element("LastName")).Contains(keyword)
                join r in receptions on (int)p.Element("Id") equals (int)r.Element("PatientId")
                join d in doctors on (int)r.Element("DoctorId") equals (int)d.Element("Id")
                orderby (int)p.Element("BirthYear") descending
                select new XElement("Patient",
                    new XAttribute("LastName", (string)p.Element("LastName")),
                    new XAttribute("BirthYear", (int)p.Element("BirthYear")),
                    new XAttribute("DoctorSpec", (string)d.Element("Specialization"))
                    )
                );
        
        }
    
    }
    class Program {
        static void Main(string[] args) {
            var dDoc = XDocument.Load("doctors.xml");
            var pDoc = XDocument.Load("patients.xml");
            var depDoc = XDocument.Load("departments.xml");
            var rDoc = XDocument.Load("receptions.xml");
            var doctors = dDoc.Descendants("Doctor");
            var patients = pDoc.Descendants("Patient");
            var departments = depDoc.Descendants("Department");
            var receptions = rDoc.Descendants("Reception");

            var report1 = MedLogic.CreateReceptionReport(doctors, patients, departments, receptions, 2024);
            report1.Save("ReceptionReport.xml");
            var report2 = MedLogic.PatientReport(patients, doctors, receptions, "a");
            report2.Save("PatientReport.xml");

        }
    
    }

}