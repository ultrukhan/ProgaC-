using ClinicSR;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace ClinicSR_Tests
{
    public class ClinicFixture {
       public IEnumerable<XElement> Patients { get; private set; }
       public IEnumerable<XElement> Doctors { get; private set; }
       public IEnumerable<XElement> Services { get; private set; }
       public IEnumerable<XElement> Visits1 { get; private set; }
       public IEnumerable<XElement> Visits2 { get; private set; }
       public IEnumerable<XElement> Visits { get; private set; }

        public ClinicFixture() {
            Patients = XElement.Parse(@"<Patients>
	<Patient>
		<Id>1</Id>
		<LastName>Litvinchuk</LastName>
		<BirthYear>2010</BirthYear>
	</Patient>
	<Patient>
		<Id>2</Id>
		<LastName>Malchevska</LastName>
		<BirthYear>2011</BirthYear>
	</Patient>
	<Patient>
		<Id>3</Id>
		<LastName>Dudchak</LastName>
		<BirthYear>2015</BirthYear>
	</Patient>
</Patients>").Descendants("Patient");
            Doctors = XElement.Parse(@"<Doctors>
	<Doctor>
		<Id>1</Id>
		<LastName>Kruvano</LastName>
		<Specialization>Surgeon</Specialization>
	</Doctor>
	<Doctor>
		<Id>2</Id>
		<LastName>Trukhan</LastName>
		<Specialization>Okulist</Specialization>
	</Doctor>
	<Doctor>
		<Id>3</Id>
		<LastName>Yaremko</LastName>
		<Specialization>Cardiolog</Specialization>
	</Doctor>
</Doctors>").Descendants("Doctor");
            Services = XElement.Parse(@"<Services>
	<Service>
		<Id>1</Id>
		<Name>S1</Name>
		<BasePrice>1500</BasePrice>
	</Service>
	<Service>
		<Id>2</Id>
		<Name>S2</Name>
		<BasePrice>1000</BasePrice>
	</Service>
	<Service>
		<Id>3</Id>
		<Name>S3</Name>
		<BasePrice>700</BasePrice>
	</Service>
</Services>").Descendants("Service");
            Visits1 = XElement.Parse(@"<Visits>
	<Visit>
		<Id>1</Id>
		<PatientId>1</PatientId>
		<DoctorId>1</DoctorId>
		<ServiceId>1</ServiceId>
		<Date>2026-06-02</Date>
		<Minutes>30</Minutes>
	</Visit>
	<Visit>
		<Id>2</Id>
		<PatientId>2</PatientId>
		<DoctorId>2</DoctorId>
		<ServiceId>2</ServiceId>
		<Date>2026-06-07</Date>
		<Minutes>40</Minutes>
	</Visit>
	<Visit>
		<Id>3</Id>
		<PatientId>3</PatientId>
		<DoctorId>3</DoctorId>
		<ServiceId>3</ServiceId>
		<Date>2026-06-10</Date>
		<Minutes>60</Minutes>
	</Visit>
</Visits>").Descendants("Visit");
             Visits2 = XElement.Parse(@"<Visits>
	<Visit>
		<Id>4</Id>
		<PatientId>1</PatientId>
		<DoctorId>3</DoctorId>
		<ServiceId>2</ServiceId>
		<Date>2026-04-06</Date>
		<Minutes>40</Minutes>
	</Visit>
	<Visit>
		<Id>5</Id>
		<PatientId>3</PatientId>
		<DoctorId>1</DoctorId>
		<ServiceId>2</ServiceId>
		<Date>2026-06-12</Date>
		<Minutes>60</Minutes>
	</Visit>
	<Visit>
		<Id>6</Id>
		<PatientId>2</PatientId>
		<DoctorId>1</DoctorId>
		<ServiceId>1</ServiceId>
		<Date>2026-06-22</Date>
		<Minutes>30</Minutes>
	</Visit>
</Visits>").Descendants("Visit");
            Visits = Visits1.Concat(Visits2);


        }
    }
    public class UnitTest1: IClassFixture<ClinicFixture>
    {
		private readonly ClinicFixture _fixture;
		public UnitTest1(ClinicFixture fixture) {
			_fixture = fixture;
		}

        [Fact]
        public void TaskATest()
        {
			var exptree = XElement.Parse(@"<TaskA Spec=""Surgeon"">
<Doctor LastName=""Kruvano"">
<Service Name=""S1"">
<Patient LastName=""Litvinchuk""/>
<Patient LastName=""Malchevska""/>
</Service>
<Service Name=""S2"">
<Patient LastName=""Dudchak""/>
</Service>
</Doctor>
</TaskA>");
			var restree = ClinicLogic.TaskA(_fixture.Patients, _fixture.Doctors, _fixture.Services, _fixture.Visits, "Surgeon");
			Assert.True(XNode.DeepEquals(exptree, restree), "Trees does not match!");

        }

        [Fact]
        public void TaskBTest()
        {
            var exptree = XElement.Parse(@"<TaskB Start=""2026-06-01T00:00:00"" End=""2026-06-15T00:00:00"">
<WeekDay Title=""Tuesday"" TotalVisit=""1"" Revenue=""1500""/>
<WeekDay Title=""Friday"" TotalVisit=""1"" Revenue=""1000""/>
<WeekDay Title=""Sunday"" TotalVisit=""1"" Revenue=""750""/>
<WeekDay Title=""Wednesday"" TotalVisit=""1"" Revenue=""700""/>
</TaskB>");
            var restree = ClinicLogic.TaskB(_fixture.Services, _fixture.Visits, new DateTime(2026, 6, 1), new DateTime(2026, 6, 15));
            Assert.True(XNode.DeepEquals(exptree, restree), "Trees does not match!");

        }
    }
}