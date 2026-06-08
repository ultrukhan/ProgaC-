using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using Procat;

namespace ProcatSRTest
{
    public class ProcatFixture {
        public IEnumerable<XElement> Offices { get; set; }
        public IEnumerable<XElement> Cars { get; set; }
        public IEnumerable<XElement> Clients { get; set; }
        public IEnumerable<XElement> Orends1 { get; set; }
        public IEnumerable<XElement> Orends2 { get; set; }
        public IEnumerable<XElement> Orends { get; set; }

        public ProcatFixture() {
            Offices = XElement.Parse(@"<Offices>
	<Office>
		<Id>1</Id>
		<Name>O1</Name>
		<City>Lviv</City>
	</Office>
	<Office>
		<Id>2</Id>
		<Name>O2</Name>
		<City>Lviv</City>
	</Office>
	<Office>
		<Id>3</Id>
		<Name>O3</Name>
		<City>Kyiv</City>
	</Office>
</Offices>").Descendants("Office");
            Cars = XElement.Parse(@"<Cars>
	<Car>
		<Id>1</Id>
		<OfficeId>1</OfficeId>
		<Mark>McLaren</Mark>
		<Class>C1</Class>
		<BasePrice>400</BasePrice>
	</Car>
	<Car>
		<Id>2</Id>
		<OfficeId>2</OfficeId>
		<Mark>Ferrari</Mark>
		<Class>C2</Class>
		<BasePrice>250</BasePrice>
	</Car>
	<Car>
		<Id>3</Id>
		<OfficeId>3</OfficeId>
		<Mark>Mercedes</Mark>
		<Class>C1</Class>
		<BasePrice>150</BasePrice>
	</Car>
	<Car>
		<Id>3</Id>
		<OfficeId>2</OfficeId>
		<Mark>Audi</Mark>
		<Class>C3</Class>
		<BasePrice>100</BasePrice>
	</Car>
</Cars>").Descendants("Car");
            Clients = XElement.Parse(@"<Clients>
	<Client>
		<Id>1</Id>
		<LastName>Yaremko</LastName>
		<Number>12345</Number>
	</Client>
	<Client>
		<Id>2</Id>
		<LastName>Kruvano</LastName>
		<Number>54321</Number>
	</Client>
	<Client>
		<Id>3</Id>
		<LastName>Trukhan</LastName>
		<Number>34521</Number>
	</Client>
</Clients>").Descendants("Client");
            Orends1 = XElement.Parse(@"<Orends>
	<Orend>
		<Id>1</Id>
		<ClientId>1</ClientId>
		<CarId>2</CarId>
		<OrDate>2026-06-01</OrDate>
		<Days>3</Days>
	</Orend>
	<Orend>
		<Id>2</Id>
		<ClientId>2</ClientId>
		<CarId>2</CarId>
		<OrDate>2026-05-29</OrDate>
		<Days>10</Days>
	</Orend>
	<Orend>
		<Id>3</Id>
		<ClientId>3</ClientId>
		<CarId>3</CarId>
		<OrDate>2026-06-05</OrDate>
		<Days>7</Days>
	</Orend>
	<Orend>
		<Id>4</Id>
		<ClientId>2</ClientId>
		<CarId>4</CarId>
		<OrDate>2026-06-15</OrDate>
		<Days>10</Days>
	</Orend>
</Orends>").Descendants("Orend");
            Orends2 = XElement.Parse(@"<Orends>
	<Orend>
		<Id>5</Id>
		<ClientId>3</ClientId>
		<CarId>1</CarId>
		<OrDate>2026-07-01</OrDate>
		<Days>5</Days>
	</Orend>
	<Orend>
		<Id>6</Id>
		<ClientId>1</ClientId>
		<CarId>4</CarId>
		<OrDate>2026-06-25</OrDate>
		<Days>10</Days>
	</Orend>
	<Orend>
		<Id>7</Id>
		<ClientId>2</ClientId>
		<CarId>3</CarId>
		<OrDate>2026-06-20</OrDate>
		<Days>8</Days>
	</Orend>
	<Orend>
		<Id>8</Id>
		<ClientId>3</ClientId>
		<CarId>1</CarId>
		<OrDate>2026-06-01</OrDate>
		<Days>3</Days>
	</Orend>
</Orends>").Descendants("Orend");
            Orends = Orends1.Concat(Orends2);

        }

    }
	public class UnitTest1 : IClassFixture<ProcatFixture>
	{
		private readonly ProcatFixture _fixture;
		public UnitTest1(ProcatFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public void TaskATest()
		{
			var exptree = XElement.Parse(@"<TaskA City=""Lviv"">
<Class Name=""C1"">
<Mark Name=""McLaren"">
<Client LastName=""Trukhan""/>
</Mark>
</Class>
<Class Name=""C2"">
<Mark Name=""Ferrari"">
<Client LastName=""Yaremko""/>
<Client LastName=""Kruvano""/>
</Mark>
</Class>
<Class Name=""C3"">
<Mark Name=""Audi"">
<Client LastName=""Trukhan""/>
<Client LastName=""Kruvano""/>
</Mark>
</Class>
</TaskA>");
			var restree = ProcatLogic.TaskA(_fixture.Offices, _fixture.Clients, _fixture.Cars, _fixture.Orends, "Lviv");
			Assert.True(XNode.DeepEquals(exptree, restree), "Trees does not match!");
		}
		[Fact]
		public void TaskBTest()
		{
			var exptree = XElement.Parse(@"<TaskB Start=""2026-06-01T00:00:00"" End=""2026-06-25T00:00:00"" MinRev=""500"">
<Office Name=""O2"" SumDays=""18"" Revenue=""2130""/>
<Office Name=""O3"" SumDays=""15"" Revenue=""2070""/>
<Office Name=""O1"" SumDays=""3"" Revenue=""1200""/>
</TaskB>");
			var restree = ProcatLogic.TaskB(_fixture.Offices, _fixture.Cars, _fixture.Orends, new DateTime(2026, 6, 1), new DateTime(2026, 6, 25), 500);
			Assert.True(XNode.DeepEquals(exptree, restree), "Trees does not match!");
		}
	}
}