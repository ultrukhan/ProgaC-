using System;
using Xunit;
using SRCinema;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
namespace CInemaTests
{
    public class CinemaFixture {
        public IEnumerable<XElement> Movies { get; private set; }
        public IEnumerable<XElement> Halls { get; private set; }
        public IEnumerable<XElement> Sessions { get; private set; }
        public IEnumerable<XElement> Tickets { get; private set; }
        public IEnumerable<XElement> Producers { get; private set; }
        public IEnumerable<XElement> Cinemas { get; private set; }
        public CinemaFixture() {
            Movies = XElement.Parse(@"
<Movies>
	<Movie>
		<Id>1</Id>
		<ProducerId>1</ProducerId>
		<Name>The Ritual</Name>	
		<Genre>Horror</Genre>
	</Movie>
	<Movie>
		<Id>2</Id>
		<ProducerId>2</ProducerId>
		<Name>Terrifier</Name>
		<Genre>Horror</Genre>
	</Movie>
	<Movie>
		<Id>3</Id>
		<ProducerId>3</ProducerId>
		<Name>Home Alone</Name>
		<Genre>Comedy</Genre>
	</Movie>
	<Movie>
		<Id>4</Id>
		<ProducerId>3</ProducerId>
		<Name>Spider Man</Name>
		<Genre>Fantasy</Genre>
	</Movie>
	<Movie>
		<Id>5</Id>
		<ProducerId>2</ProducerId>
		<Name>Doctor Strange</Name>
		<Genre>Fantasy</Genre>
	</Movie>
</Movies>
").Descendants("Movie");
			Halls = XElement.Parse(@"
<Halls>
	<Hall>
		<Id>1</Id>
		<CinemaId>1</CinemaId>
		<Name>Hall1</Name>
		<Capacity>70</Capacity>
	</Hall>
	<Hall>
		<Id>2</Id>
		<CinemaId>2</CinemaId>
		<Name>Hall2</Name>
		<Capacity>90</Capacity>
	</Hall>
	<Hall>
		<Id>3</Id>
		<CinemaId>3</CinemaId>
		<Name>Hall3</Name>
		<Capacity>60</Capacity>
	</Hall>
	<Hall>
		<Id>4</Id>
		<CinemaId>1</CinemaId>
		<Name>Hall4</Name>
		<Capacity>60</Capacity>
	</Hall>
</Halls>
").Descendants("Hall");
			Producers = XElement.Parse(@"
<Producers>
	<Producer>
		<Id>1</Id>
		<LastName>Trukhan</LastName>
		<Country>Ukraine</Country>
	</Producer>
	<Producer>
		<Id>2</Id>
		<LastName>Kruvano</LastName>
		<Country>Ukraine</Country>
	</Producer>
	<Producer>
		<Id>3</Id>
		<LastName>Yaremko</LastName>
		<Country>Spain</Country>
	</Producer>
</Producers>
").Descendants("Producer");
			Sessions = XElement.Parse(@"
<Sessions>
	<Session>
		<Id>1</Id>
		<MovieId>1</MovieId>
		<HallId>1</HallId>
		<Date>2026-05-01</Date>
	</Session>
	<Session>
		<Id>2</Id>
		<MovieId>1</MovieId>
		<HallId>2</HallId>
		<Date>2026-04-29</Date>
	</Session>
	<Session>
		<Id>3</Id>
		<MovieId>2</MovieId>
		<HallId>3</HallId>
		<Date>2026-05-02</Date>
	</Session>
	<Session>
		<Id>4</Id>
		<MovieId>4</MovieId>
		<HallId>4</HallId>
		<Date>2026-05-02</Date>
	</Session>
	<Session>
		<Id>5</Id>
		<MovieId>5</MovieId>
		<HallId>2</HallId>
		<Date>2026-05-03</Date>
	</Session>
	<Session>
		<Id>6</Id>
		<MovieId>3</MovieId>
		<HallId>3</HallId>
		<Date>2026-04-28</Date>
	</Session>
</Sessions>
").Descendants("Session");
			Tickets = XElement.Parse(@"
<Tickets>
	<Ticket>
		<Id>1</Id>
		<SessionId>1</SessionId>
		<Price>100</Price>
	</Ticket>
	<Ticket>
		<Id>2</Id>
		<SessionId>2</SessionId>
		<Price>100</Price>
	</Ticket>
	<Ticket>
		<Id>3</Id>
		<SessionId>3</SessionId>
		<Price>120</Price>
	</Ticket>
	<Ticket>
		<Id>4</Id>
		<SessionId>4</SessionId>
		<Price>130</Price>
	</Ticket>
	<Ticket>
		<Id>5</Id>
		<SessionId>5</SessionId>
		<Price>150</Price>
	</Ticket>
	<Ticket>
		<Id>6</Id>
		<SessionId>6</SessionId>
		<Price>120</Price>
	</Ticket>
	<Ticket>
		<Id>7</Id>
		<SessionId>1</SessionId>
		<Price>200</Price>
	</Ticket>
	<Ticket>
		<Id>8</Id>
		<SessionId>2</SessionId>
		<Price>110</Price>
	</Ticket>
	<Ticket>
		<Id>9</Id>
		<SessionId>4</SessionId>
		<Price>100</Price>
	</Ticket>
	<Ticket>
		<Id>10</Id>
		<SessionId>4</SessionId>
		<Price>100</Price>
	</Ticket>
	<Ticket>
		<Id>11</Id>
		<SessionId>5</SessionId>
		<Price>150</Price>
	</Ticket>
	<Ticket>
		<Id>12</Id>
		<SessionId>5</SessionId>
		<Price>120</Price>
	</Ticket>
</Tickets>
").Descendants("Ticket");
			Cinemas = XElement.Parse(@"
<Cinemas>
	<Cinema>
		<Id>1</Id>
		<Name>Cinema1</Name>
		<City>Lviv</City>
	</Cinema>
	<Cinema>
		<Id>2</Id>
		<Name>Cinema2</Name>
		<City>Kyiv</City>
	</Cinema>
	<Cinema>
		<Id>3</Id>
		<Name>Cinema3</Name>
		<City>Lviv</City>
	</Cinema>
</Cinemas>
").Descendants("Cinema");
        }
    }
    public class UnitTest1: IClassFixture<CinemaFixture>
    {
		private readonly CinemaFixture _fixture;
		public UnitTest1(CinemaFixture fixture)
		{
			_fixture = fixture;
        }

        [Fact]
        public void Test1()
        {
			var exptree = XElement.Parse(@"
<CinemasReport City='Kyiv'>
<Cinema CName='Cinema2'>
<Producer PName='Kruvano'>
<Movie MName='Doctor Strange'/>
</Producer>
<Producer PName='Trukhan'>
<Movie MName='The Ritual'/>
</Producer>
</Cinema>
</CinemasReport>");
			var result = CinemaLogic.TaskA(_fixture.Movies, _fixture.Halls, _fixture.Sessions, _fixture.Tickets, _fixture.Producers, _fixture.Cinemas, "Kyiv");
			Assert.True(XNode.DeepEquals(exptree, result), "Xml trees do not match");
        }
		[Fact]
		public void Test2() {
			var exptree2 = XElement.Parse(@"
<RevenueByGenre Start=""2026-04-29T00:00:00"" End=""2026-05-03T00:00:00"" MinRev=""500"">
<Genre Name=""Horror"" TicketCount=""5"" Revenue=""630""/>
<Genre Name=""Fantasy"" TicketCount=""6"" Revenue=""750""/>
</RevenueByGenre>
");
			var result2 = CinemaLogic.TaskB(_fixture.Movies, _fixture.Sessions, _fixture.Tickets, new DateTime(2026, 4, 29), new DateTime(2026, 5, 3), 500);
			Assert.True(XNode.DeepEquals(exptree2, result2), "Xml trees do not match");
        }
    }
}