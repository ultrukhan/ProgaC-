using Pidgot_5;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace TestPidgot_5
{
    public class Fixture
    {
        public IEnumerable<XElement> Tovars { get; private set; }
        public IEnumerable<XElement> Clients { get; private set; }
        public IEnumerable<XElement> Categories { get; private set; }
        public IEnumerable<XElement> Histories1 { get; private set; }
        public IEnumerable<XElement> Histories2 { get; private set; }
        public IEnumerable<XElement> Histories { get; private set; }
        public Fixture()
        {
            Tovars = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<Tovar>
	<Tov>
		<T_id>1</T_id>
		<Ca_id>1</Ca_id>
		<Name>Tov1</Name>
		<Price>100</Price>
	</Tov>
	<Tov>
		<T_id>2</T_id>
		<Ca_id>2</Ca_id>
		<Name>Tov2</Name>
		<Price>200</Price>
	</Tov>
	<Tov>
		<T_id>3</T_id>
		<Ca_id>3</Ca_id>
		<Name>Tov3</Name>
		<Price>300</Price>
	</Tov>
</Tovar>").Descendants("Tov");
            Clients = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<Clients>
	<Cli>
		<Cl_id>1</Cl_id>
		<Surname>Sur1</Surname>
		<Numder>0978097500</Numder>
		<Mail>ul.trukhan07@gmail.com</Mail>
	</Cli>
	<Cli>
		<Cl_id>2</Cl_id>
		<Surname>Sur2</Surname>
		<Numder>0975119984</Numder>
	</Cli>
	<Cli>
		<Cl_id>3</Cl_id>
		<Surname>Sur3</Surname>
		<Mail>meprodrawler@gmail.com</Mail>
	</Cli>
	<Cli>
		<Cl_id>4</Cl_id>
		<Surname>Sur4</Surname>
	</Cli>
</Clients>").Descendants("Cli");
            Categories = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<Categories>
	<Cat>
		<Ca_id>1</Ca_id>
		<Title>Category1</Title>
	</Cat>
	<Cat>
		<Ca_id>2</Ca_id>
		<Title>Category2</Title>
	</Cat>
	<Cat>
		<Ca_id>3</Ca_id>
		<Title>Category3</Title>
	</Cat>
</Categories>").Descendants("Cat");
            Histories1 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Histories>
	<Hist>
		<Date>2026-01-01</Date>
		<Z_id>1</Z_id>
		<Cl_id>1</Cl_id>
		<T_id>1</T_id>
		<Num>1</Num>
	</Hist>
	<Hist>
		<Date>2026-01-01</Date>
		<Z_id>2</Z_id>
		<Cl_id>1</Cl_id>
		<T_id>1</T_id>
		<Num>5</Num>
	</Hist>
	<Hist>
		<Date>2026-01-01</Date>
		<Z_id>3</Z_id>
		<Cl_id>2</Cl_id>
		<T_id>2</T_id>
		<Num>10</Num>
	</Hist>
	<Hist>
		<Date>2026-05-05</Date>
		<Z_id>4</Z_id>
		<Cl_id>3</Cl_id>
		<T_id>3</T_id>
		<Num>4</Num>
	</Hist>
</Histories>").Descendants("Hist");
            Histories2 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Histories>
	<Hist>
		<Date>2026-02-02</Date>
		<Z_id>5</Z_id>
		<Cl_id>4</Cl_id>
		<T_id>1</T_id>
		<Num>4</Num>
	</Hist>
	<Hist>
		<Date>2026-03-03</Date>
		<Z_id>6</Z_id>
		<Cl_id>1</Cl_id>
		<T_id>2</T_id>
		<Num>20</Num>
	</Hist>
	<Hist>
		<Date>2026-06-06</Date>
		<Z_id>7</Z_id>
		<Cl_id>2</Cl_id>
		<T_id>3</T_id>
		<Num>12</Num>
	</Hist>
	<Hist>
		<Date>2026-05-05</Date>
		<Z_id>8</Z_id>
		<Cl_id>3</Cl_id>
		<T_id>1</T_id>
		<Num>2</Num>
	</Hist>
</Histories>").Descendants("Hist");
            Histories = Histories1.Concat(Histories2);
        }
    }
    public class UnitTest1 : IClassFixture<Fixture>
    {
        public readonly Fixture _fixture;
        public UnitTest1(Fixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void TestTaskA()
        {
            var exp = XElement.Parse(@"<TaskA>
<Client Surname=""Sur2"" Total=""4480"" Contact=""0975119984""/>
<Client Surname=""Sur1"" Total=""3700"" Contact=""0978097500""/>
<Client Surname=""Sur3"" Total=""1400"" Contact=""meprodrawler@gmail.com""/>
<Client Surname=""Sur4"" Total=""400"" Contact=""Unknown""/>
</TaskA>");
            var res = Logic.TaskA(_fixture.Tovars, _fixture.Clients, _fixture.Histories);
            Assert.True(XNode.DeepEquals(exp, res), "Trees not the same");
        }
        [Fact]
        public void TestTaskB()
        {
            var exp = XElement.Parse(@"<TaskB>
<Category Title=""Category1"">
<Zamovlenias Zamovlenia=""2"" Total=""5""/>
<Zamovlenias Zamovlenia=""5"" Total=""4""/>
<Zamovlenias Zamovlenia=""8"" Total=""2""/>
<Zamovlenias Zamovlenia=""1"" Total=""1""/>
</Category>
<Category Title=""Category2"">
<Zamovlenias Zamovlenia=""6"" Total=""20""/>
<Zamovlenias Zamovlenia=""3"" Total=""10""/>
</Category>
<Category Title=""Category3"">
<Zamovlenias Zamovlenia=""7"" Total=""12""/>
<Zamovlenias Zamovlenia=""4"" Total=""4""/>
</Category>
</TaskB>");
            var res = Logic.TaskB(_fixture.Tovars, _fixture.Clients, _fixture.Categories, _fixture.Histories);
            Assert.True(XNode.DeepEquals(exp, res), "Trees not the same");
        }
    }
}
