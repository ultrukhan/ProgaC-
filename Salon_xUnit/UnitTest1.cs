using System;
using Xunit;
using System.Collections.Generic;
using System.Xml.Linq;
using SalonTask;

namespace Salon_xUnit
{
    public class SalonDataFixture {
        public IEnumerable<XElement> Cars { get; private set; }
        public IEnumerable<XElement> Brands { get; private set; }
        public IEnumerable<XElement> Menegers { get; private set; }
        public IEnumerable<XElement> Buyments { get; private set; }

        public SalonDataFixture() {
            Menegers = XDocument.Load("menegers.xml").Descendants("Meneger");
            Brands = XDocument.Load("brands.xml").Descendants("Brand");
            Cars = XDocument.Load("cars.xml").Descendants("Car");
            Buyments = XDocument.Load("buyments.xml").Descendants("Buyment");
        }
    }
    public class UnitTest1: IClassFixture<SalonDataFixture>
    {
        private readonly SalonDataFixture _fixture;
        public UnitTest1(SalonDataFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(3,"Kruvano",4)]
        [InlineData(1,"Trukhan",5)]
        public void Test1(int expc,string last,int month)
        {
            var result = SalonLogic.CreateMonthReport(_fixture.Menegers, _fixture.Brands, _fixture.Cars, _fixture.Buyments, month);
            var count = result.Elements("Meneger").Count();
            Assert.Equal(expc, count);
            var surname = (string)result.Elements("Meneger").First().Attribute("LastName");
            Assert.Equal(last, surname);
        }
        [Theory]
        [InlineData(3,"Italy",5000)]
        [InlineData(1,"Italy",10000)]
        public void Test2(int expc, string country, int minrev) {
            var result = SalonLogic.CreateCountryStatistic(_fixture.Brands, _fixture.Cars, _fixture.Buyments, minrev);
            var count = result.Elements("Country").Count();
            Assert.Equal(expc, count);
            var countryname = (string)result.Elements("Country").First().Attribute("Name");
            Assert.Equal(country, countryname);
        }
    }
    public class MenegerTest {
        [Theory]
        [InlineData(10, "Blabla", 10)]
        [InlineData(15, "Blblb", 45)]
        public void Correct_data_setter_test(int id, string ln, int st) {
            var obj = new Meneger(1, "test1", 5);
            obj.Id = id;
            obj.LastName = ln;
            obj.Stage = st;
            Assert.Equal(id, obj.Id);
            Assert.Equal(ln, obj.LastName);
            Assert.Equal(st, obj.Stage);
        }
        [Fact]
        public void InCorrect_LastName_setter_test() {
            var ob2 = new Meneger(12, "test2", 11);
            Assert.Throws<ArgumentException>(() => ob2.LastName = "");
        }
        [Fact]
        public void InCorresct_Stage_setter_test() {
            var ob3 = new Meneger(9, "test3", 9);
            Assert.Throws<ArgumentException>(() => ob3.Stage = -19);
        }
    }
}