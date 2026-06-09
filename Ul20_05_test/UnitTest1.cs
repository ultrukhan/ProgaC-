using SR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ObsTest
{
    public class ObsFixture
    {
        public IEnumerable<XElement> Astronoms { get; private set; }
        public IEnumerable<XElement> CObjs { get; private set; }
        public IEnumerable<XElement> RTels { get; private set; }
        public IEnumerable<XElement> Seanss { get; private set; }

        public ObsFixture()
        {
            Astronoms = XElement.Parse(@"<Astronoms>
	<Astronom>
		<AstrId>1</AstrId>
		<Surname>Myzuchyk</Surname>
		<Stupin>Docent</Stupin>
	</Astronom>
	<Astronom>
		<AstrId>2</AstrId>
		<Surname>Holovatuy</Surname>
		<Stupin>Profesor</Stupin>
	</Astronom>
	<Astronom>
		<AstrId>3</AstrId>
		<Surname>Tarasyk</Surname>
		<Stupin>Docent</Stupin>
	</Astronom>
</Astronoms>").Descendants("Astronom");
            CObjs = XElement.Parse(@"<CObjs>
	<CObj>
		<CObjId>1</CObjId>
		<Name>Object1</Name>
		<Price>100</Price>
	</CObj>
	<CObj>
		<CObjId>2</CObjId>
		<Name>Object2</Name>
		<Price>200</Price>
	</CObj>
	<CObj>
		<CObjId>3</CObjId>
		<Name>Object3</Name>
		<Price>300</Price>
	</CObj>
</CObjs>").Descendants("CObj");
            RTels = XElement.Parse(@"<RTels>
	<RTel>
		<RTelId>1</RTelId>
		<Title>Teleskop1</Title>
		<Diametr>3</Diametr>
	</RTel>
	<RTel>
		<RTelId>2</RTelId>
		<Title>Teleskop2</Title>
		<Diametr>5</Diametr>
	</RTel>
	<RTel>
		<RTelId>3</RTelId>
		<Title>Teleskop3</Title>
		<Diametr>7</Diametr>
	</RTel>
</RTels>").Descendants("RTel");
            Seanss = XElement.Parse(@"<Seanss>
	<Seans>
		<Date>2025-04-04</Date>
		<Truv>1</Truv>
		<CObjId>1</CObjId>
		<AstrId>1</AstrId>
		<RTelId>1</RTelId>
	</Seans>
	<Seans>
		<Date>2025-05-05</Date>
		<Truv>2</Truv>
		<CObjId>2</CObjId>
		<AstrId>2</AstrId>
		<RTelId>2</RTelId>
	</Seans>
	<Seans>
		<Date>2025-07-07</Date>
		<Truv>3</Truv>
		<CObjId>3</CObjId>
		<AstrId>3</AstrId>
		<RTelId>3</RTelId>
	</Seans>
	<Seans>
		<Date>2025-04-04</Date>
		<Truv>5</Truv>
		<CObjId>1</CObjId>
		<AstrId>1</AstrId>
		<RTelId>1</RTelId>
	</Seans>
</Seanss>").Descendants("Seans");
        }
    }
    public class UnitTest1 : IClassFixture<ObsFixture>
    {
        private readonly ObsFixture _fixture;
        public UnitTest1(ObsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TaskATest()
        {
            var exptree = XElement.Parse(@"<TaskA>
  <ByObject Object=""Object1"">
    <Stats Astronom=""Myzuchyk"" Time=""6"" />
  </ByObject>
  <ByObject Object=""Object2"">
    <Stats Astronom=""Holovatuy"" Time=""2"" />
  </ByObject>
  <ByObject Object=""Object3"">
    <Stats Astronom=""Tarasyk"" Time=""3"" />
  </ByObject>
</TaskA>");
            var result = ObservLogic.TaskA(_fixture.CObjs, _fixture.Astronoms, _fixture.Seanss);
            Assert.True(XNode.DeepEquals(exptree, result));
        }
        [Fact]
        public void TaskBTest()
        {
            var exptree = XElement.Parse(@"<TaskB>
  <Astronom Name=""Holovatuy"">
    <Teleskope Month=""5"" Name=""Teleskop2"" Spent=""400"" />
  </Astronom>
  <Astronom Name=""Myzuchyk"">
    <Teleskope Month=""4"" Name=""Teleskop1"" Spent=""600"" />
  </Astronom>
  <Astronom Name=""Tarasyk"">
    <Teleskope Month=""7"" Name=""Teleskop3"" Spent=""900"" />
  </Astronom>
</TaskB>");
            var result = ObservLogic.TaskB(_fixture.CObjs, _fixture.Astronoms, _fixture.Seanss, _fixture.RTels);
            Assert.True(XNode.DeepEquals(exptree, result));
        }
    }
}