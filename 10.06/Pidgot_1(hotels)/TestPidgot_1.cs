using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Pidgot_1;

namespace Pidgot_1Test
{
	public class HotelFixture
	{
		public IEnumerable<XElement> Hotels { get; private set; }
		public IEnumerable<XElement> Guests { get; private set; }
		public IEnumerable<XElement> Categories { get; private set; }
		public IEnumerable<XElement> Datas1 { get; private set; }
		public IEnumerable<XElement> Datas2 { get; private set; }
		public IEnumerable<XElement> Datas { get; private set; }

		public HotelFixture()
		{
			Hotels = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Hotels>
	<Hotel>
		<H_id>1</H_id>
		<H_name>Hotel1</H_name>
		<H_city>Lviv</H_city>
	</Hotel>
	<Hotel>
		<H_id>2</H_id>
		<H_name>Hotel2</H_name>
		<H_city>Kyiv</H_city>
	</Hotel>
	<Hotel>
		<H_id>3</H_id>
		<H_name>Hotel3</H_name>
		<H_city>Odesa</H_city>
	</Hotel>
</Hotels>").Descendants("Hotel");
			Guests = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Guests>
	<Guest>
		<G_id>1</G_id>
		<Surname>Trukhan</Surname>
		<Number>0978097500</Number>
	</Guest>
	<Guest>
		<G_id>2</G_id>
		<Surname>Forti</Surname>
		<Number>0682525358</Number>
	</Guest>
	<Guest>
		<G_id>3</G_id>
		<Surname>Kruvano</Surname>
		<Number>0986332629</Number>
	</Guest>
</Guests>").Descendants("Guest");
			Categories = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Categories>
	<Categorie>
		<C_id>1</C_id>
		<H_id>1</H_id>
		<C_name>Lux</C_name>
		<Price>800</Price>
	</Categorie>
	<Categorie>
		<C_id>2</C_id>
		<H_id>2</H_id>
		<C_name>Standart</C_name>
		<Price>300</Price>
	</Categorie>
	<Categorie>
		<C_id>3</C_id>
		<H_id>3</H_id>
		<C_name>Category3</C_name>
		<Price>400</Price>
	</Categorie>
	<Categorie>
		<C_id>4</C_id>
		<H_id>2</H_id>
		<C_name>Lux+</C_name>
		<Price>1000</Price>
	</Categorie>
	<Categorie>
		<C_id>5</C_id>
		<H_id>3</H_id>
		<C_name>Standart</C_name>
		<Price>300</Price>
	</Categorie>
	<Categorie>
		<C_id>6</C_id>
		<H_id>1</H_id>
		<C_name>Category6</C_name>
		<Price>400</Price>
	</Categorie>
</Categories>").Descendants("Categorie");
			Datas1 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Datas>
	<Data>
		<D_id>1</D_id>
		<G_id>1</G_id>
		<C_id>1</C_id>
		<B_date>2026-01-01</B_date>
		<Z_date>2026-06-06</Z_date>
		<Nights>7</Nights>
	</Data>
	<Data>
		<D_id>2</D_id>
		<G_id>2</G_id>
		<C_id>3</C_id>
		<B_date>2026-06-01</B_date>
		<Z_date>2026-06-06</Z_date>
		<Nights>3</Nights>
	</Data>
	<Data>
		<D_id>3</D_id>
		<G_id>3</G_id>
		<C_id>2</C_id>
		<B_date>2026-05-05</B_date>
		<Z_date>2026-06-01</Z_date>
		<Nights>4</Nights>
	</Data>
	<Data>
		<D_id>4</D_id>
		<G_id>1</G_id>
		<C_id>4</C_id>
		<B_date>2026-06-01</B_date>
		<Z_date>2026-06-06</Z_date>
		<Nights>1</Nights>
	</Data>
</Datas>").Descendants("Data");
			Datas2 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Datas>
	<Data>
		<D_id>5</D_id>
		<G_id>2</G_id>
		<C_id>5</C_id>
		<B_date>2026-02-10</B_date>
		<Z_date>2026-09-04</Z_date>
		<Nights>5</Nights>
	</Data>
	<Data>
		<D_id>6</D_id>
		<G_id>3</G_id>
		<C_id>6</C_id>
		<B_date>2026-01-01</B_date>
		<Z_date>2026-01-03</Z_date>
		<Nights>3</Nights>
	</Data>
	<Data>
		<D_id>7</D_id>
		<G_id>3</G_id>
		<C_id>3</C_id>
		<B_date>2026-06-01</B_date>
		<Z_date>2026-06-02</Z_date>
		<Nights>4</Nights>
	</Data>
	<Data>
		<D_id>8</D_id>
		<G_id>1</G_id>
		<C_id>2</C_id>
		<B_date>2026-03-03</B_date>
		<Z_date>2026-09-04</Z_date>
		<Nights>4</Nights>
	</Data>
</Datas>").Descendants("Data");
			Datas = Datas1.Concat(Datas2);


		}
	}
	public class UnitTest1 : IClassFixture<HotelFixture>
	{
		private readonly HotelFixture _fixture;

		public UnitTest1(HotelFixture fixture)
		{
			_fixture = fixture;
		}
		[Fact]
		public void TaskATest()
		{
			var exptree = XElement.Parse(@"<TaskA City=""Lviv"">
  <Client Surname=""Kruvano"">
    <Hotel Title=""Hotel1"">
      <Category Name=""Category6"" />
    </Hotel>
  </Client>
  <Client Surname=""Trukhan"">
    <Hotel Title=""Hotel1"">
      <Category Name=""Lux"" />
    </Hotel>
  </Client>
</TaskA>");
			var res = HotLogic.TaskA(_fixture.Datas, _fixture.Hotels, _fixture.Guests, _fixture.Categories, "Lviv");
			Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");
		}
		[Fact]
		public void TeskBTest()
		{
			var exptree = XElement.Parse(@"<TaskB From=""2026-01-01T00:00:00"" To=""2026-06-06T00:00:00"" minimal_dohid=""0"">
  <Hotel Name=""Hotel1"" Nights=""10"" Total=""7920"" />
  <Hotel Name=""Hotel3"" Nights=""7"" Total=""2800"" />
  <Hotel Name=""Hotel2"" Nights=""5"" Total=""2440"" />
</TaskB>");
			var res = HotLogic.TaskB(_fixture.Datas, _fixture.Hotels, _fixture.Categories, new DateTime(2026, 01, 01), new DateTime(2026, 06, 06), 0.0);
			Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");

		}
	}
}