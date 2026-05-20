using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SRRest;

namespace RestorantTest
{
    public class RestorantFixture {
        public IEnumerable<XElement> Restorants { get; private set; }
        public IEnumerable<XElement> Clients { get; private set; }
        public IEnumerable<XElement> Dishes { get; private set; }
        public IEnumerable<XElement> Deliv1 { get; private set; }
        public IEnumerable<XElement> Deliv2 { get; private set; }
        public IEnumerable<XElement> Deliveries { get; private set; }

        public RestorantFixture()
        {
            Restorants = XElement.Parse(@"<Restorants>
	<Restorant>
		<Id>1</Id>
		<Name>Res1</Name>
		<City>Lviv</City>
	</Restorant>
	<Restorant>
		<Id>2</Id>
		<Name>Res2</Name>
		<City>Kyiv</City>
	</Restorant>
	<Restorant>
		<Id>3</Id>
		<Name>Res3</Name>
		<City>Lviv</City>
	</Restorant>
</Restorants> ").Descendants("Restorant");
            Clients = XElement.Parse(@"<Clients>
	<Client>
		<Id>1</Id>
		<LastName>Trukhan</LastName>
		<PhoneNumber>+38074527423</PhoneNumber>
	</Client>
	<Client>
		<Id>2</Id>
		<LastName>Kruvano</LastName>
		<PhoneNumber>+380976451734</PhoneNumber>
	</Client>
	<Client>
		<Id>3</Id>
		<LastName>Yaremko</LastName>
		<PhoneNumber>+3809811234334</PhoneNumber>
	</Client>
</Clients> ").Descendants("Client");
            Dishes = XElement.Parse(@"<Dishes>
	<Dish>
		<Id>1</Id>
		<Name>D1</Name>
		<Category>C1</Category>
		<Price>150</Price>
	</Dish>
	<Dish>
		<Id>2</Id>
		<Name>D2</Name>
		<Category>C2</Category>
		<Price>200</Price>
	</Dish>
	<Dish>
		<Id>3</Id>
		<Name>D3</Name>
		<Category>C1</Category>
		<Price>300</Price>
	</Dish>
</Dishes> ").Descendants("Dish");
            Deliv1 = XElement.Parse(@"<Deliveries>
	<Delivery>
		<Id>1</Id>
		<RestorantId>1</RestorantId>
		<ClientId>1</ClientId>
		<DishId>1</DishId>
		<Date>2026-05-20</Date>
		<NumOf>2</NumOf>
	</Delivery>
	<Delivery>
		<Id>2</Id>
		<RestorantId>2</RestorantId>
		<ClientId>2</ClientId>
		<DishId>2</DishId>
		<Date>2026-05-19</Date>
		<NumOf>4</NumOf>
	</Delivery>
	<Delivery>
		<Id>3</Id>
		<RestorantId>3</RestorantId>
		<ClientId>3</ClientId>
		<DishId>3</DishId>
		<Date>2026-05-22</Date>
		<NumOf>2</NumOf>
	</Delivery>
	<Delivery>
		<Id>4</Id>
		<RestorantId>2</RestorantId>
		<ClientId>1</ClientId>
		<DishId>3</DishId>
		<Date>2026-05-15</Date>
		<NumOf>2</NumOf>
	</Delivery>
</Deliveries> ").Descendants("Delivery");
            Deliv2 = XElement.Parse(@"<Deliveries>
	<Delivery>
		<Id>5</Id>
		<RestorantId>1</RestorantId>
		<ClientId>3</ClientId>
		<DishId>2</DishId>
		<Date>2026-04-22</Date>
		<NumOf>2</NumOf>
	</Delivery>
	<Delivery>
		<Id>6</Id>
		<RestorantId>3</RestorantId>
		<ClientId>1</ClientId>
		<DishId>1</DishId>
		<Date>2026-05-27</Date>
		<NumOf>1</NumOf>
	</Delivery>
	<Delivery>
		<Id>7</Id>
		<RestorantId>1</RestorantId>
		<ClientId>2</ClientId>
		<DishId>1</DishId>
		<Date>2026-05-21</Date>
		<NumOf>2</NumOf>
	</Delivery>
	<Delivery>
		<Id>8</Id>
		<RestorantId>2</RestorantId>
		<ClientId>1</ClientId>
		<DishId>3</DishId>
		<Date>2026-05-10</Date>
		<NumOf>3</NumOf>
	</Delivery>
</Deliveries> ").Descendants("Delivery");
            Deliveries = Deliv1.Concat(Deliv2);


        }
    }
    public class UnitTest1 : IClassFixture<RestorantFixture>
    {
		private readonly RestorantFixture _fixture;

		public UnitTest1(RestorantFixture fixture) {
			_fixture = fixture;
        }
        [Fact]
        public void TaskATest()
        {
			var exptree = XElement.Parse(@"<TaskA City=""Lviv"">
<Client LastName=""Kruvano"">
<Category CName=""C1"">
<Dish DName=""D1""/>
</Category>
</Client>
<Client LastName=""Trukhan"">
<Category CName=""C1"">
<Dish DName=""D1""/>
</Category>
</Client>
<Client LastName=""Yaremko"">
<Category CName=""C1"">
<Dish DName=""D3""/>
</Category>
<Category CName=""C2"">
<Dish DName=""D2""/>
</Category>
</Client>
</TaskA>");
			var res = RestorantLogic.TaskA(_fixture.Restorants, _fixture.Clients, _fixture.Dishes,_fixture.Deliveries, "Lviv");
			Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");
        }
        [Fact]
        public void TeskBTest()
        {
            var exptree = XElement.Parse(@"<TaskB Start=""2026-05-10T00:00:00"" End=""2026-05-25T00:00:00"" minRev=""500"">
<Category Name=""C1"" DelivNum=""5"" Revenue=""2700""/>
<Category Name=""C2"" DelivNum=""1"" Revenue=""800""/>
</TaskB>");
            var res = RestorantLogic.TaskB(_fixture.Dishes,_fixture.Deliveries, new DateTime(2026, 5, 10), new DateTime(2026, 5, 25), 500);
            Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");

        }
    }
}