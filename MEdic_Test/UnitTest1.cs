using System.Xml.Linq;
using Xunit;
using MedProg;
using System.Collections.Generic;
using System.Linq;
using System.Net;

// подати без повторень = додаткове групування 
namespace MEdic_Test
{
    public class UnitTest1
    {
        public static (IEnumerable<XElement> doctors, IEnumerable<XElement> patients, IEnumerable<XElement> departments, IEnumerable<XElement> receptions) GetData() {
            var doctors = XDocument.Load("doctors.xml").Descendants("Doctor");
            var patients = XDocument.Load("patients.xml").Descendants("Patient");
            var departments = XDocument.Load("departments.xml").Descendants("Department");
            var receptions = XDocument.Load("receptions.xml").Descendants("Reception");

            return (doctors, patients, departments, receptions);
        }
        [Theory]
        [InlineData(2025,2,"Dep3")]
        [InlineData(2024,1,"Dep1")]
        public void Task1_correct_test(int year,int expcount,string expdep)
        {
            var (doctors, patients, departments, receptions) = GetData();
            var report = MedLogic.CreateReceptionReport(doctors, patients, departments, receptions, year);
            var count = report.Elements("Department").Count();
            Assert.Equal(expcount, count);
            var dep = (string)report.Elements("Department").First().Attribute("Name");
            Assert.Equal(expdep, dep);
        }
        [Theory]
        [InlineData("u",2015,4)]
        [InlineData("a",2015,5)]
        public void Task2_correct_test(string keyw, int expyear, int expcount) {
            var (doctors, patients, departments, receptions) = GetData();
            var report = MedLogic.PatientReport(patients, doctors, receptions, keyw);
            var count = report.Elements("Patient").Count();
            Assert.Equal(expcount, count);
            var year = (int)report.Elements("Patient").First().Attribute("BirthYear");
            Assert.Equal(expyear, year);


        }
    }
}