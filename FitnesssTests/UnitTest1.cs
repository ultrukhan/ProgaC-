using FitnessSr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FitnesssTests
{
    public class FitnessFixture {
        public IEnumerable<XElement> Clubs { get; private set; }
        public IEnumerable<XElement> Clients { get; private set; }
        public IEnumerable<XElement> Coaches { get; private set; }
        public IEnumerable<XElement> Workouts { get; private set; }

        public FitnessFixture() {
            Clubs = XElement.Parse(@"<Clubs>
	<Club>
		<Id>1</Id>
		<Name>GymMax</Name>
		<City>Kyiv</City>
	</Club>
	<Club>
		<Id>2</Id>
		<Name>FitLine</Name>
		<City>Lviv</City>
	</Club>
</Clubs>").Descendants("Club");
            Clients = XElement.Parse(@"<Clients>
	<Client>
		<Id>10</Id>
		<LastName>Yaremko</LastName>
	</Client>
	<Client>
		<Id>11</Id>
		<LastName>Kruvano</LastName>
	</Client>
	<Client>
		<Id>12</Id>
		<LastName>Trukhan</LastName>
	</Client>
</Clients>").Descendants("Client");
            Coaches = XElement.Parse(@"<Coaches>
	<Coach>
		<Id>100</Id>
		<LastName>Malchevska</LastName>
		<Role>Yoga</Role>
	</Coach>
	<Coach>
		<Id>101</Id>
		<LastName>Litvinchuk</LastName>
		<Role>Crossfit</Role>
	</Coach>
	<Coach>
		<Id>102</Id>
		<LastName>Stetsiv</LastName>
		<Role>Boks</Role>
	</Coach>
</Coaches>").Descendants("Coach");
            Workouts = XElement.Parse(@"<Workouts>
	<Workout>
		<Id>1</Id>
		<ClubId>1</ClubId>
		<ClientId>10</ClientId>
		<CoachId>100</CoachId>
		<Date>2026-05-01</Date>
		<Price>500</Price>
	</Workout>
	<Workout>
		<Id>2</Id>
		<ClubId>1</ClubId>
		<ClientId>10</ClientId>
		<CoachId>100</CoachId>
		<Date>2026-05-02</Date>
		<Price>500</Price>
	</Workout>
	<Workout>
		<Id>3</Id>
		<ClubId>1</ClubId>
		<ClientId>10</ClientId>
		<CoachId>101</CoachId>
		<Date>2026-05-02</Date>
		<Price>600</Price>
	</Workout>

	<Workout>
		<Id>4</Id>
		<ClubId>2</ClubId>
		<ClientId>11</ClientId>
		<CoachId>102</CoachId>
		<Date>2026-05-04</Date>
		<Price>800</Price>
	</Workout>
	<Workout>
		<Id>5</Id>
		<ClubId>2</ClubId>
		<ClientId>11</ClientId>
		<CoachId>101</CoachId>
		<Date>2026-04-25</Date>
		<Price>900</Price>
	</Workout>

	<Workout>
		<Id>6</Id>
		<ClubId>1</ClubId>
		<ClientId>12</ClientId>
		<CoachId>100</CoachId>
		<Date>2026-05-05</Date>
		<Price>700</Price>
	</Workout>
</Workouts>").Descendants("Workout");
        }
    }
    public class UnitTest1 : IClassFixture<FitnessFixture>
    {
		private readonly FitnessFixture _fixture;
		public UnitTest1(FitnessFixture fixture) {
			_fixture = fixture;
        }

        [Fact]
        public void TaskATest()
        {
			var exptree = XElement.Parse(@"<CityClubReport City=""Kyiv"">
<Club Name=""GymMax"">
<CoachRole Role=""Crossfit"">
<Coach LastName=""Litvinchuk""/>
</CoachRole>
<CoachRole Role=""Yoga"">
<Coach LastName=""Malchevska""/>
</CoachRole>
</Club>
</CityClubReport>");
			var result = FitnessLogic.TaskA(_fixture.Clubs, _fixture.Clients, _fixture.Coaches, _fixture.Workouts, "Kyiv");
			Assert.True(XNode.DeepEquals(exptree, result));
        }
        [Fact]
        public void TaskBTest()
        {
            var exptree = XElement.Parse(@"<RoleRevenueReport StartDate=""2026-05-01T00:00:00"" EndDate=""2026-05-03T00:00:00"" MinRev=""100"">
<CoachRole Role=""Yoga"" Workouts=""2"" Revenue=""1000""/>
<CoachRole Role=""Crossfit"" Workouts=""1"" Revenue=""600""/>
</RoleRevenueReport>");
            var result = FitnessLogic.TaskB(_fixture.Coaches, _fixture.Workouts, new DateTime(2026, 5, 1), new DateTime(2026, 5, 3), 100);
            Assert.True(XNode.DeepEquals(exptree, result));
        }
    }
}