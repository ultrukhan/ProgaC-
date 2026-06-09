using Pidgot_4; // змінити
using System;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Collections.Generic;

namespace TestPidgot_4
{
    public class FLFixture
    {
        public IEnumerable<XElement> Destinations { get; private set; }
        public IEnumerable<XElement> Passengers { get; private set; }
        public IEnumerable<XElement> Flights { get; private set; }
        public IEnumerable<XElement> Tic1 { get; private set; }
        public IEnumerable<XElement> Tic2 { get; private set; }
        public IEnumerable<XElement> Tics { get; private set; }

        public FLFixture()
        {
            Destinations = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Destinations>
	<Destination>
		<Id>1</Id>
		<Name>Київ</Name>
		<PricePerFlight>1000</PricePerFlight>
	</Destination>
	<Destination>
		<Id>2</Id>
		<Name>Варшава</Name>
		<PricePerFlight>2500</PricePerFlight>
	</Destination>
	<Destination>
		<Id>3</Id>
		<Name>Лондон</Name>
		<PricePerFlight>5000</PricePerFlight>
	</Destination>
</Destinations>").Descendants("Destination");
            Passengers = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Passengers>
	<Passenger>
		<Id>1</Id>
		<LastName>Шевченко</LastName>
		<Age>30</Age>
	</Passenger>
	<Passenger>
		<Id>2</Id>
		<LastName>Коваленко</LastName>
		<Age>45</Age>
	</Passenger>
	<Passenger>
		<Id>3</Id>
		<LastName>Бойко</LastName>
		<Age>22</Age>
	</Passenger>
	<Passenger>
		<Id>4</Id>
		<LastName>Мельник</LastName>
		<Age>50</Age>
	</Passenger>
</Passengers>").Descendants("Passenger");
            Flights = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Flights>
	<Flight>
		<Id>101</Id>
		<DestinationId>1</DestinationId>
		<AirplaneModel>Boeing 737</AirplaneModel>
	</Flight>
	<Flight>
		<Id>102</Id>
		<DestinationId>2</DestinationId>
		<AirplaneModel>Airbus A320</AirplaneModel>
	</Flight>
	<Flight>
		<Id>103</Id>
		<DestinationId>3</DestinationId>
		<AirplaneModel>Boeing 737</AirplaneModel>
	</Flight>
	<Flight>
		<Id>104</Id>
		<DestinationId>2</DestinationId>
		<AirplaneModel>Boeing 737</AirplaneModel>
	</Flight>
</Flights>").Descendants("Flight");
            Tic1 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Tickets>
	<Ticket>
		<FlightId>101</FlightId>
		<PassengerId>1</PassengerId>
		<Date>2024-01-15</Date>
	</Ticket>
	<Ticket>
		<FlightId>102</FlightId>
		<PassengerId>1</PassengerId>
		<Date>2024-01-20</Date>
	</Ticket>
	<Ticket>
		<FlightId>103</FlightId>
		<PassengerId>2</PassengerId>
		<Date>2024-01-18</Date>
	</Ticket>
	<Ticket>
		<FlightId>101</FlightId>
		<PassengerId>3</PassengerId>
		<Date>2024-02-10</Date>
	</Ticket>
	<Ticket>
		<FlightId>103</FlightId>
		<PassengerId>4</PassengerId>
		<Date>2024-02-15</Date>
	</Ticket>
</Tickets>").Descendants("Ticket");
            Tic2 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Tickets>
	<Ticket>
		<FlightId>104</FlightId>
		<PassengerId>2</PassengerId>
		<Date>2024-02-28</Date>
	</Ticket>
	<Ticket>
		<FlightId>103</FlightId>
		<PassengerId>1</PassengerId>
		<Date>2024-02-12</Date>
	</Ticket>
	<Ticket>
		<FlightId>102</FlightId>
		<PassengerId>3</PassengerId>
		<Date>2024-03-05</Date>
	</Ticket>
	<Ticket>
		<FlightId>104</FlightId>
		<PassengerId>4</PassengerId>
		<Date>2024-03-10</Date>
	</Ticket>
	<Ticket>
		<FlightId>101</FlightId>
		<PassengerId>4</PassengerId>
		<Date>2024-03-12</Date>
	</Ticket>
</Tickets>").Descendants("Ticket");
            Tics = Tic1.Concat(Tic2);
        }
    }
    public class UnitTest1 : IClassFixture<FLFixture>
    {
        public readonly FLFixture _fixture;
        public UnitTest1(FLFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void TaskATest()
        {
            var exp = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TaskA>
  <Reys Id=""102"" Sum=""5000"" />
  <Reys Id=""104"" Sum=""5000"" />
  <Reys Id=""101"" Sum=""3000"" />
  <Reys Id=""103"" Sum=""15000"" />
</TaskA>");
            var res = FlLogic.TaskA(_fixture.Destinations, _fixture.Flights, _fixture.Tics);
            Assert.True(XNode.DeepEquals(exp, res), "Trees don`t match!");
        }
        [Fact]
        public void TaskBTest()
        {
            var exp = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TaskB>
  <Passanger Surname=""Бойко"">
    <Statistics Punkt=""Варшава"" Paid=""2500"" />
  </Passanger>
  <Passanger Surname=""Коваленко"">
    <Statistics Punkt=""Лондон"" Paid=""5000"" />
  </Passanger>
  <Passanger Surname=""Мельник"">
    <Statistics Punkt=""Лондон"" Paid=""5000"" />
  </Passanger>
  <Passanger Surname=""Шевченко"">
    <Statistics Punkt=""Лондон"" Paid=""5000"" />
  </Passanger>
</TaskB>");
            var res = FlLogic.TaskB(_fixture.Destinations, _fixture.Passengers, _fixture.Flights, _fixture.Tics);
            Assert.True(XNode.DeepEquals(exp, res), "Trees don`t match!");
        }
    }
}