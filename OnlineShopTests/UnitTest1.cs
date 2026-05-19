using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OnlineShopSR;

namespace OnlineShopTests
{
    public class OnlineShopFixture {
        public IEnumerable<XElement> Categories { get; private set; }
        public IEnumerable<XElement> Clients { get; private set; }
        public IEnumerable<XElement> Products { get; private set; }
        public IEnumerable<XElement> Orders { get; private set; }

        public OnlineShopFixture() {
            Categories = XElement.Parse(@"<Categories>
	<Category>
		<Id>1</Id>
		<Name>Tech</Name>
	</Category>
	<Category>
		<Id>2</Id>
		<Name>Food</Name>
	</Category>
	<Category>
		<Id>3</Id>
		<Name>Clothes</Name>
	</Category>
</Categories>").Descendants("Category");
            Clients = XElement.Parse(@"<Clients>
	<Client>
		<Id>1</Id>
		<LastName>Yaremko</LastName>
		<City>Lviv</City>
	</Client>
	<Client>
		<Id>2</Id>
		<LastName>Kruvano</LastName>
		<City>Lviv</City>
	</Client>
	<Client>
		<Id>3</Id>
		<LastName>Trukhan</LastName>
		<City>Kyiv</City>
	</Client>
</Clients>").Descendants("Client");
            Products = XElement.Parse(@"<Products>
	<Product>
		<Id>1</Id>
		<Name>Apple</Name>
		<CategoryId>2</CategoryId>
		<Price>50</Price>
	</Product>
	<Product>
		<Id>2</Id>
		<Name>Laptop</Name>
		<CategoryId>1</CategoryId>
		<Price>2000</Price>
	</Product>
	<Product>
		<Id>3</Id>
		<Name>Sweater</Name>
		<CategoryId>3</CategoryId>
		<Price>300</Price>
	</Product>
</Products>").Descendants("Product");
            Orders = XElement.Parse(@"<Orders>
	<Order>
		<Id>1</Id>
		<ClientId>1</ClientId>
		<ProductId>1</ProductId>
		<Date>2026-05-01</Date>
		<NumOf>10</NumOf>
	</Order>
	<Order>
		<Id>2</Id>
		<ClientId>2</ClientId>
		<ProductId>2</ProductId>
		<Date>2026-05-01</Date>
		<NumOf>1</NumOf>
	</Order>
	<Order>
		<Id>3</Id>
		<ClientId>3</ClientId>
		<ProductId>3</ProductId>
		<Date>2026-05-02</Date>
		<NumOf>2</NumOf>
	</Order>
	<Order>
		<Id>4</Id>
		<ClientId>2</ClientId>
		<ProductId>1</ProductId>
		<Date>2026-05-03</Date>
		<NumOf>5</NumOf>
	</Order>
	<Order>
		<Id>6</Id>
		<ClientId>1</ClientId>
		<ProductId>3</ProductId>
		<Date>2026-04-29</Date>
		<NumOf>1</NumOf>
	</Order>
	<Order>
		<Id>7</Id>
		<ClientId>3</ClientId>
		<ProductId>1</ProductId>
		<Date>2026-05-02</Date>
		<NumOf>10</NumOf>
	</Order>
</Orders>").Descendants("Order");
        }
    }
    public class UnitTest1 : IClassFixture<OnlineShopFixture>
    {
		private readonly OnlineShopFixture _fixture;
		public UnitTest1(OnlineShopFixture fixture) {
			_fixture = fixture;
		}

        [Fact]
        public void TaskATest()
        {
			var exptree = XElement.Parse(@"<CityReport City=""Lviv"">
<Client LastName=""Kruvano"">
<Category Name=""Food"">
<Product Name=""Apple""/>
</Category>
<Category Name=""Tech"">
<Product Name=""Laptop""/>
</Category>
</Client>
<Client LastName=""Yaremko"">
<Category Name=""Clothes"">
<Product Name=""Sweater""/>
</Category>
<Category Name=""Food"">
<Product Name=""Apple""/>
</Category>
</Client>
</CityReport>");
			var res = OnlineShopLogic.TaskA(_fixture.Categories, _fixture.Clients,_fixture.Products,_fixture.Orders, "Lviv");
			Assert.True(XNode.DeepEquals(exptree, res),"Trees doesn`t match!");

        }
        [Fact]
        public void TaskBTest()
        {
            var exptree = XElement.Parse(@"<CategoryRevenueReport StartDate=""2026-05-01T00:00:00"" EndDate=""2026-05-04T00:00:00"" MinRev=""500"">
<Category Name=""Tech"" NumOfOrdres=""1"" Revenue=""2000""/>
<Category Name=""Food"" NumOfOrdres=""3"" Revenue=""1250""/>
<Category Name=""Clothes"" NumOfOrdres=""1"" Revenue=""600""/>
</CategoryRevenueReport>");
            var res = OnlineShopLogic.TaskB(_fixture.Categories,_fixture.Products,_fixture.Orders, new DateTime(2026, 5, 1), new DateTime(2026, 5, 4), 500);
            Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");
        }
    }
}