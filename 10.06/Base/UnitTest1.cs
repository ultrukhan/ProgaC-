using Pidgot_4; // змінити
using System;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Collections.Generic;

namespace TestPidgot_4
{
    public class Fixture
    {
        public IEnumerable<XElement>  { get; private set; }
        public IEnumerable<XElement>  { get; private set; }
        public IEnumerable<XElement>  { get; private set; }
        public IEnumerable<XElement>  { get; private set; }
        public IEnumerable<XElement>  { get; private set; }
        public IEnumerable<XElement>  { get; private set; }

        public Fixture()
        {
             = XElement.Parse("@").Descendants("");
             = XElement.Parse("@").Descendants("");
             = XElement.Parse("@").Descendants("");
             = XElement.Parse("@").Descendants("");
             = XElement.Parse("@").Descendants("");

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
        public void TaskATest()
        {
            var exp = XElement.Parse("@");
            var res = Logic.TaskA();
            Assert.True(XNode.DeepEquals(exp, res), "Trees don`t match!");
        }
        [Fact]
        public void TaskBTest()
        {
            var exp = XElement.Parse("@");
            var res = Logic.TaskB();
            Assert.True(XNode.DeepEquals(exp, res), "Trees don`t match!");
        }
    }
}