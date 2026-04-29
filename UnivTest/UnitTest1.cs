using System.Xml.Linq;
using Xunit;
using UnivProgram;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UnivTest
{
    public class UNIVTEST
    {
        public static (IEnumerable<XElement> teachers,IEnumerable<XElement> courses,IEnumerable<XElement> topics,IEnumerable<XElement> schedules) GetData() {
            var teachers = XDocument.Load("test_teachers.xml").Descendants("Teacher");
            var courses = XDocument.Load("test_courses.xml").Descendants("Course");
            var topics = XDocument.Load("test_topics.xml").Descendants("Topic");
            var schedules = XDocument.Load("test_schedules.xml").Descendants("Schedule");
            return ( teachers, courses, topics, schedules );
        }
        [Theory]
        [InlineData(1,3,23)]
        [InlineData(2,1,21)]
        public void Report_correct(int sem,int expcount,int descgroup)
        {
            var (teachers, courses, topics, schedules) = GetData();
            var result = UnivLogic.GenereteSemesterReport(teachers, courses, topics, schedules, sem);
            var count = result.Elements("GroupReport").Count();
            Assert.Equal(expcount, count);
            var group = (int)result.Elements("GroupReport").First().Attribute("GroupNumber");
            Assert.Equal(descgroup,group);
        }
        [Theory]
        [InlineData("API", 2)]
        [InlineData("Verbs",2)]
        [InlineData("al",4)]
        public void TeacherReport_correct(string search,int expcount) {
            var (teachers, courses, topics, schedules) = GetData();
            var result = UnivLogic.RightTeacherReport(teachers, topics, schedules, search);
            var count = result.Elements("TeachersReport").Count();
            Assert.Equal(expcount, count);
        }
    }
}