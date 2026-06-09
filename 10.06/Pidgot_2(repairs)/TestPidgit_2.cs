using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Pidgot_1;

namespace Pidgot_1Test
{
    public class AutFixture
    {
        public IEnumerable<XElement> Ofises { get; private set; }
        public IEnumerable<XElement> Clients { get; private set; }
        public IEnumerable<XElement> Avtos { get; private set; }
        public IEnumerable<XElement> Datas1 { get; private set; }
        public IEnumerable<XElement> Datas2 { get; private set; }
        public IEnumerable<XElement> Datas { get; private set; }

        public AutFixture()
        {
            Ofises = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Ofiss>
	<Ofis>
		<O_id>1</O_id>
		<Name>Ofise1</Name>
		<City>Lviv</City>
	</Ofis>
	<Ofis>
		<O_id>2</O_id>
		<Name>Ofise2</Name>
		<City>Kyiv</City>
	</Ofis>
	<Ofis>
		<O_id>3</O_id>
		<Name>Ofise3</Name>
		<City>Dnipro</City>
	</Ofis>
	<Ofis>
		<O_id>4</O_id>
		<Name>Ofise4</Name>
		<City>Odesa</City>
	</Ofis>
</Ofiss>").Descendants("Ofis");
            Clients = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Clients>
	<Client>
		<C_id>1</C_id>
		<Sur>Trukhan</Sur>
		<Number>1234567890</Number>
	</Client>
	<Client>
		<C_id>2</C_id>
		<Sur>Kruvano</Sur>
		<Number>0987654321</Number>
	</Client>
	<Client>
		<C_id>3</C_id>
		<Sur>Forti</Sur>
		<Number>1234509876</Number>
	</Client>
</Clients>").Descendants("Client");
            Avtos = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Avtos>
	<Avto>
		<A_id>1</A_id>
		<O_id>4</O_id>
		<Marka>Audi</Marka>
		<Class>Econom</Class>
		<BasePrice>1500</BasePrice>
	</Avto>
	<Avto>
		<A_id>2</A_id>
		<O_id>2</O_id>
		<Marka>Marsedes</Marka>
		<Class>Busines</Class>
		<BasePrice>5000</BasePrice>
	</Avto>
	<Avto>
		<A_id>3</A_id>
		<O_id>3</O_id>
		<Marka>Shkoda</Marka>
		<Class>Econom</Class>
		<BasePrice>1000</BasePrice>
	</Avto>
	<Avto>
		<A_id>4</A_id>
		<O_id>1</O_id>
		<Marka>Maclaren</Marka>
		<Class>Busines</Class>
		<BasePrice>5000</BasePrice>
	</Avto>
</Avtos>").Descendants("Avto");
            Datas1 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Datas>
	<Data>
		<D_id>1</D_id>
		<C_id>1</C_id>
		<A_id>1</A_id>
		<StartDate>2026-01-01</StartDate>
		<Days>3</Days>
	</Data>
	<Data>
		<D_id>2</D_id>
		<C_id>2</C_id>
		<A_id>4</A_id>
		<StartDate>2026-06-06</StartDate>
		<Days>5</Days>
	</Data>
	<Data>
		<D_id>3</D_id>
		<C_id>3</C_id>
		<A_id>3</A_id>
		<StartDate>2026-06-04</StartDate>
		<Days>1</Days>
	</Data>
	<Data>
		<D_id>4</D_id>
		<C_id>1</C_id>
		<A_id>2</A_id>
		<StartDate>2026-05-05</StartDate>
		<Days>2</Days>
	</Data>
	<Data>
		<D_id>5</D_id>
		<C_id>2</C_id>
		<A_id>1</A_id>
		<StartDate>2026-04-07</StartDate>
		<Days>12</Days>
	</Data>
</Datas>").Descendants("Data");
            Datas2 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Datas>
	<Data>
		<D_id>6</D_id>
		<C_id>3</C_id>
		<A_id>2</A_id>
		<StartDate>2026-01-08</StartDate>
		<Days>12</Days>
	</Data>
	<Data>
		<D_id>7</D_id>
		<C_id>1</C_id>
		<A_id>4</A_id>
		<StartDate>2026-02-06</StartDate>
		<Days>5</Days>
	</Data>
	<Data>
		<D_id>8</D_id>
		<C_id>2</C_id>
		<A_id>3</A_id>
		<StartDate>2026-06-01</StartDate>
		<Days>10</Days>
	</Data>
	<Data>
		<D_id>9</D_id>
		<C_id>3</C_id>
		<A_id>2</A_id>
		<StartDate>2026-05-12</StartDate>
		<Days>4</Days>
	</Data>
	<Data>
		<D_id>10</D_id>
		<C_id>1</C_id>
		<A_id>1</A_id>
		<StartDate>2025-12-07</StartDate>
		<Days>8</Days>
	</Data>
</Datas>").Descendants("Data");
            Datas = Datas1.Concat(Datas2);
        }
    }
    public class UnitTest1 : IClassFixture<AutFixture>
    {
        private readonly AutFixture _fixture;

        public UnitTest1(AutFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void TaskATest()
        {
            var exptree = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TaskA>
  <Car Class=""Busines"">
    <Details Marka=""Maclaren"">
      <Client Surname=""Kruvano"" />
      <Client Surname=""Trukhan"" />
    </Details>
  </Car>
</TaskA>");
            var res = AutLogic.TaskA(_fixture.Datas, _fixture.Ofises, _fixture.Clients, _fixture.Avtos, "Lviv");
            Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");
        }
        [Fact]
        public void TeskBTest()
        {
            var exptree = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TaskB>
  <Ofise Name=""Ofise4"" Days=""15"" Dohid=""25200"" />
  <Ofise Name=""Ofise1"" Days=""10"" Dohid=""50000"" />
  <Ofise Name=""Ofise2"" Days=""18"" Dohid=""99000"" />
</TaskB>");
            var res = AutLogic.TaskB(_fixture.Datas, _fixture.Ofises, _fixture.Avtos, new DateTime(2026, 01, 01), new DateTime(2026, 06, 06), 13000);
            Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");

        }
    }
}