using MedProg;
using MedProg.MedProg;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Xunit;

namespace MEdic_Test
{
    public class MedicDataFixture
    {
        public IEnumerable<XElement> Doctors { get; private set; }
        public IEnumerable<XElement> Patients { get; private set; }
        public IEnumerable<XElement> Departments { get; private set; }
        public IEnumerable<XElement> Receptions { get; private set; }

        public MedicDataFixture()
        {
            Doctors = XDocument.Load("doctors.xml").Descendants("Doctor");
            Patients = XDocument.Load("patients.xml").Descendants("Patient");
            Departments = XDocument.Load("departments.xml").Descendants("Department");
            Receptions = XDocument.Load("receptions.xml").Descendants("Reception");
        }
    }
    public class UnitTest1 : IClassFixture<MedicDataFixture>
    {
        private readonly MedicDataFixture _fixture;

        public UnitTest1(MedicDataFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(2025,2,"Dep3")]
        [InlineData(2024,1,"Dep1")]
        public void Task1_correct_test(int year,int expcount,string expdep)
        {
            var report = MedLogic.CreateReceptionReport(_fixture.Doctors, _fixture.Patients, _fixture.Departments, _fixture.Receptions, year);
            var count = report.Elements("Department").Count();
            Assert.Equal(expcount, count);
            var dep = (string)report.Elements("Department").First().Attribute("Name");
            Assert.Equal(expdep, dep);
        }
        [Theory]
        [InlineData("u",2015,4)]
        [InlineData("a",2015,5)]
        public void Task2_correct_test(string keyw, int expyear, int expcount) {
            var report = MedLogic.PatientReport(_fixture.Patients, _fixture.Doctors, _fixture.Receptions, keyw);
            var count = report.Elements("Patient").Count();
            Assert.Equal(expcount, count);
            var year = (int)report.Elements("Patient").First().Attribute("BirthYear");
            Assert.Equal(expyear, year);


        }
    }
    public class PatientTests
    {
        [Theory]
        [InlineData(101,"t1",40)]
        [InlineData(102,"t2",22)]
        public void Patient_SetProperties_ReturnsCorrectValues(int id,string ln, int age)
        {
            var patient = new Patient();

            patient.Id = id;
            patient.LastName = ln;
            patient.Age = age;

            Assert.Equal(id, patient.Id);
            Assert.Equal(ln, patient.LastName);
            Assert.Equal(age, patient.Age);
        }

        [Fact]
        public void Patient_AgeSetter_ThrowsException_IfNegative()
        {
            // Arrange
            var patient = new Patient();

            // Act & Assert (Для помилок це робиться в один рядок)
            // Ми кажемо xUnit: "Перевір, чи вилетить ArgumentException, якщо я зроблю ось це"
            Assert.Throws<ArgumentException>(() => patient.Age = -5);
        }

        // ТЕСТ 3: Перевіряємо, що сетер прізвища не пускає пусті рядки
        [Theory]
        [InlineData("")]
        [InlineData("   ")] // Лише пробіли
        [InlineData(null)]
        public void Patient_LastNameSetter_ThrowsException_IfEmpty(string badName)
        {
            var patient = new Patient();

            // Перевіряємо, що на всі погані дані з [InlineData] вилітає помилка
            Assert.Throws<ArgumentException>(() => patient.LastName = badName);
        }
    }
}