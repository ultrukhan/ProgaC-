using Pidgot_3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Pidgot_1Test
{
    public class HotelFixture
    {
        public IEnumerable<XElement> Patients { get; private set; }
        public IEnumerable<XElement> Doctors { get; private set; }
        public IEnumerable<XElement> Poslugs { get; private set; }
        public IEnumerable<XElement> Datas1 { get; private set; }
        public IEnumerable<XElement> Datas2 { get; private set; }
        public IEnumerable<XElement> Datas { get; private set; }

        public HotelFixture()
        {
            Patients = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<Patients>
	<Patient>
		<P_id>1</P_id>
		<Surname>Trukhan</Surname>
		<BirthDate>2007-09-04</BirthDate>
	</Patient>
	<Patient>
		<P_id>2</P_id>
		<Surname>Kruvano</Surname>
		<BirthDate>2007-08-02</BirthDate>
	</Patient>
	<Patient>
		<P_id>3</P_id>
		<Surname>Forti</Surname>
		<BirthDate>2010-07-13</BirthDate>
	</Patient>
</Patients>").Descendants("Patient");
            Doctors = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Doctors>
	<Doctor>
		<D_id>1</D_id>
		<Surname>Fok</Surname>
		<Spetialization>Surger</Spetialization>
	</Doctor>
	<Doctor>
		<D_id>2</D_id>
		<Surname>Zavadka</Surname>
		<Spetialization>Oftalmologist</Spetialization>
	</Doctor>
	<Doctor>
		<D_id>3</D_id>
		<Surname>Forti</Surname>
		<Spetialization>Nurse</Spetialization>
	</Doctor>
</Doctors>").Descendants("Doctor");
            Poslugs = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Poslugas>
	<Posluga>
		<Po_id>1</Po_id>
		<Title>Checkup</Title>
		<BasePrice>500</BasePrice>
	</Posluga>
	<Posluga>
		<Po_id>2</Po_id>
		<Title>Operation</Title>
		<BasePrice>3000</BasePrice>
	</Posluga>
</Poslugas>").Descendants("Posluga");
            Datas1 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Datas>
	<Data>
		<V_id>1</V_id>
		<P_id>1</P_id>
		<D_id>1</D_id>
		<Po_id>1</Po_id>
		<Date>2026-01-01</Date>
		<Time>30</Time>
	</Data>
	<Data>
		<V_id>2</V_id>
		<P_id>2</P_id>
		<D_id>2</D_id>
		<Po_id>2</Po_id>
		<Date>2026-06-01</Date>
		<Time>300</Time>
	</Data>
	<Data>
		<V_id>3</V_id>
		<P_id>3</P_id>
		<D_id>3</D_id>
		<Po_id>1</Po_id>
		<Date>2026-06-06</Date>
		<Time>45</Time>
	</Data>
	<Data>
		<V_id>4</V_id>
		<P_id>1</P_id>
		<D_id>2</D_id>
		<Po_id>1</Po_id>
		<Date>2026-05-01</Date>
		<Time>15</Time>
	</Data>
</Datas>").Descendants("Data");
            Datas2 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Datas>
	<Data>
		<V_id>5</V_id>
		<P_id>2</P_id>
		<D_id>3</D_id>
		<Po_id>2</Po_id>
		<Date>2026-05-09</Date>
		<Time>240</Time>
	</Data>
	<Data>
		<V_id>6</V_id>
		<P_id>3</P_id>
		<D_id>1</D_id>
		<Po_id>1</Po_id>
		<Date>2026-05-09</Date>
		<Time>20</Time>
	</Data>
	<Data>
		<V_id>7</V_id>
		<P_id>1</P_id>
		<D_id>3</D_id>
		<Po_id>2</Po_id>
		<Date>2026-06-09</Date>
		<Time>240</Time>
	</Data>
	<Data>
		<V_id>8</V_id>
		<P_id>2</P_id>
		<D_id>2</D_id>
		<Po_id>1</Po_id>
		<Date>2026-02-09</Date>
		<Time>50</Time>
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
            var exptree = XElement.Parse(@"<TaskA>
<Doctor Surname=""Fok"">
<Posluga Title=""Checkup"">
<Patient Surname=""Trukhan""/>
<Patient Surname=""Forti""/>
</Posluga>
</Doctor>
</TaskA>");
            var res = LicarLogic.TaskA(_fixture.Patients, _fixture.Doctors, _fixture.Poslugs, _fixture.Datas, "Surger");
            Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");
        }
        [Fact]
        public void TeskBTest()
        {
            var exptree = XElement.Parse(@"<TaskB>
<Date Day=""Monday"" Visits=""2"" Paid=""3500""/>
<Date Day=""Saturday"" Visits=""3"" Paid=""3000""/>
<Date Day=""Thursday"" Visits=""1"" Paid=""500""/>
<Date Day=""Friday"" Visits=""1"" Paid=""500""/>
</TaskB>");
            var res = LicarLogic.TaskB(_fixture.Poslugs, _fixture.Datas, new DateTime(2026, 01, 01), new DateTime(2026, 06, 06));
            Assert.True(XNode.DeepEquals(exptree, res), "Trees doesn`t match!");

        }
    }
}