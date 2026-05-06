using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace UnivProgram {
    public static class UnivLogic {
        public static XElement GenereteSemesterReport(IEnumerable<XElement> teachers, IEnumerable<XElement> courses, IEnumerable<XElement> topics, IEnumerable<XElement> schedules,int semester) 
        {
            var flatData = (from s in schedules
                            where (int)s.Element("Semester") == semester
                            join c in courses on (int)s.Element("CourseId") equals (int)c.Element("Id")
                            join t in topics on (int)s.Element("TopicId") equals (int)t.Element("Id")
                            join tc in teachers on (int)t.Element("TeacherId") equals (int)tc.Element("Id")
                            select new {
                                Group = (int)s.Element("GroupNumber"),
                                Course = (string)c.Element("Title"),
                                Topic = (string)t.Element("Title"),
                                Teacher = (string)tc.Element("LastName")
                            });
            return new XElement("SemesterReport", new XAttribute("Semester",semester),
                from d in flatData 
                orderby d.Group descending
                group d by d.Group into g
                select new XElement("GroupReport",
                    new XAttribute("GroupNumber",g.Key),
                    from dg in g
                    group dg by dg.Course into gc
                    select new XElement("Course",
                        new XAttribute("Title",gc.Key),
                        from gce in gc
                        orderby gce.Teacher
                        select new XElement("Topic",
                            new XAttribute("Teacher",gce.Teacher),
                            new XAttribute("Title",gce.Topic)
                            )
                        )
                    )
                );
           
        }
        public static XElement RightTeacherReport(IEnumerable<XElement> teachers, IEnumerable<XElement> topics,IEnumerable<XElement>schedules, string keyword) {
            return new XElement("RightTeacherReport", new XAttribute("Keyword", keyword),
                from t in teachers
                join tp in topics on (int)t.Element("Id") equals (int)tp.Element("TeacherId")
                where ((string)tp.Element("Title")).Contains(keyword)
                join s in schedules on (int)tp.Element("Id") equals (int)s.Element("TopicId")
                orderby (string)t.Element("LastName")
                select new XElement("TeachersReport",
                    new XAttribute("Teacher", (string)t.Element("LastName")),
                    new XAttribute("Semester", (int)s.Element("Semester")),
                    new XAttribute("GroupNumber", (int)s.Element("GroupNumber")))
                );
        }
    
    }
    class Program {
        static void Main(string[] args) {
            var tDoc = XDocument.Load("teachers.xml");
            var cDoc = XDocument.Load("courses.xml");
            var tpDoc = XDocument.Load("topics.xml");
            var sDoc = XDocument.Load("schedules.xml");
            var teachers = tDoc.Descendants("Teacher");
            var courses = cDoc.Descendants("Course");
            var topics = tpDoc.Descendants("Topic");
            var schedules = sDoc.Descendants("Schedule");

            var report = UnivLogic.GenereteSemesterReport(teachers, courses, topics, schedules, 1);
            report.Save("SemesterReport.xml");

            var rtreport = UnivLogic.RightTeacherReport(teachers, topics, schedules, "al");
            rtreport.Save("RightTeacherReport.xml");
        }
    }
}