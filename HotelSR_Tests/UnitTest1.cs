using HotelSR;
using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace HotelSR_Tests
{
    public class HotelFixture {
        public IEnumerable<XElement> Hotels { get; private set; }
        public IEnumerable<XElement> Guests { get; private set; }
        public IEnumerable<XElement> Categs { get; private set; }
        public IEnumerable<XElement> Brons1 { get; private set; }
        public IEnumerable<XElement> Brons2 { get; private set; }
        public IEnumerable<XElement> Brons { get; private set; }

        public HotelFixture() {
            Hotels = XElement.Parse(@"<Hotels>
	<Hotel>
		<Id>1</Id>
		<Name>H1</Name>
		<City>Lviv</City>
	</Hotel>
	<Hotel>
		<Id>2</Id>
		<Name>H2</Name>
		<City>Lviv</City>
	</Hotel>
	<Hotel>
		<Id>3</Id>
		<Name>H3</Name>
		<City>Kyiv</City>
	</Hotel>
</Hotels>").Descendants("Hotel");
            Guests = XElement.Parse(@"<Guests>
	<Guest>
		<Id>1</Id>
		<LastName>Kruvano</LastName>
		<Phone>+3809858238</Phone>
	</Guest>
	<Guest>
		<Id>2</Id>
		<LastName>Trukhan</LastName>
		<Phone>+3809883841</Phone>
	</Guest>
	<Guest>
		<Id>3</Id>
		<LastName>Litvinchuk</LastName>
		<Phone>+3809814839</Phone>
	</Guest>
</Guests>").Descendants("Guest");
            Categs = XElement.Parse(@"<Categs>
	<Categ>
		<Id>1</Id>
		<HotelId>1</HotelId>
		<Name>L</Name>
		<BasePrice>150</BasePrice>
	</Categ>
	<Categ>
		<Id>2</Id>
		<HotelId>2</HotelId>
		<Name>M</Name>
		<BasePrice>200</BasePrice>
	</Categ>
	<Categ>
		<Id>3</Id>
		<HotelId>3</HotelId>
		<Name>L</Name>
		<BasePrice>250</BasePrice>
	</Categ>
	<Categ>
		<Id>4</Id>
		<HotelId>2</HotelId>
		<Name>VIP</Name>
		<BasePrice>400</BasePrice>
	</Categ>
</Categs>").Descendants("Categ");
            Brons1 = XElement.Parse(@"<Brons>
	<Bron>
		<Id>1</Id>
		<GuestId>1</GuestId>
		<CategId>1</CategId>
		<BDate>2026-06-01</BDate>
		<ZDate>2026-06-20</ZDate>
		<Nights>3</Nights>
	</Bron>
	<Bron>
		<Id>2</Id>
		<GuestId>2</GuestId>
		<CategId>2</CategId>
		<BDate>2026-06-01</BDate>
		<ZDate>2026-06-10</ZDate>
		<Nights>4</Nights>
	</Bron>
	<Bron>
		<Id>3</Id>
		<GuestId>3</GuestId>
		<CategId>3</CategId>
		<BDate>2026-06-05</BDate>
		<ZDate>2026-06-22</ZDate>
		<Nights>4</Nights>
	</Bron>
	<Bron>
		<Id>4</Id>
		<GuestId>1</GuestId>
		<CategId>4</CategId>
		<BDate>2026-05-29</BDate>
		<ZDate>2026-06-07</ZDate>
		<Nights>2</Nights>
	</Bron>
</Brons>").Descendants("Bron");
            Brons2 = XElement.Parse(@"<Brons>
	<Bron>
		<Id>5</Id>
		<GuestId>1</GuestId>
		<CategId>4</CategId>
		<BDate>2026-06-05</BDate>
		<ZDate>2026-06-20</ZDate>
		<Nights>3</Nights>
	</Bron>
	<Bron>
		<Id>6</Id>
		<GuestId>3</GuestId>
		<CategId>2</CategId>
		<BDate>2026-05-25</BDate>
		<ZDate>2026-06-05</ZDate>
		<Nights>4</Nights>
	</Bron>
	<Bron>
		<Id>7</Id>
		<GuestId>1</GuestId>
		<CategId>3</CategId>
		<BDate>2026-06-10</BDate>
		<ZDate>2026-06-24</ZDate>
		<Nights>4</Nights>
	</Bron>
	<Bron>
		<Id>8</Id>
		<GuestId>2</GuestId>
		<CategId>4</CategId>
		<BDate>2026-05-25</BDate>
		<ZDate>2026-06-25</ZDate>
		<Nights>2</Nights>
	</Bron>
</Brons>").Descendants("Bron");
            Brons = Brons1.Concat(Brons2);

        }


    }
    public class UnitTest1 : IClassFixture<HotelFixture>
    {
		private readonly HotelFixture _fixture;
		public UnitTest1(HotelFixture fixture) {
			_fixture = fixture;
		}

        [Fact]
        public void TaskATest()
        {
			var exptree = XElement.Parse(@"<TaskA City=""Lviv"">
<Guest LastName=""Kruvano"">
<Hotel Name=""H1"">
<Category Name=""L""/>
</Hotel>
<Hotel Name=""H2"">
<Category Name=""VIP""/>
</Hotel>
</Guest>
<Guest LastName=""Litvinchuk"">
<Hotel Name=""H2"">
<Category Name=""M""/>
</Hotel>
</Guest>
<Guest LastName=""Trukhan"">
<Hotel Name=""H2"">
<Category Name=""M""/>
<Category Name=""VIP""/>
</Hotel>
</Guest>
</TaskA>");

			var restree = HotelLogic.TaskA(_fixture.Hotels, _fixture.Guests, _fixture.Categs, _fixture.Brons, "Lviv");
			Assert.True(XNode.DeepEquals(exptree, restree), "Trees does not match!");
        }
        [Fact]
        public void TaskBTest()
        {
            var exptree = XElement.Parse(@"<TaskB Start=""2025-06-01T00:00:00"" End=""2026-07-20T00:00:00"" MinRev=""200"">
<Hotel Name=""H2"" NumOfB=""5"" Revenue=""4200""/>
<Hotel Name=""H3"" NumOfB=""2"" Revenue=""1900""/>
<Hotel Name=""H1"" NumOfB=""1"" Revenue=""405""/>
</TaskB>");

            var restree = HotelLogic.TaskB(_fixture.Hotels, _fixture.Categs, _fixture.Brons, new DateTime(2025, 6, 1), new DateTime(2026, 7, 20), 200);
            Assert.True(XNode.DeepEquals(exptree, restree), "Trees does not match!");
        }
    }
}