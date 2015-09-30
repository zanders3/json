using JsonParser;
using System.Collections.Generic;
using NUnit.Framework;

namespace JsonParserTests
{
    [TestFixture]
    public class TestWriter
    {
        [Test]
        public void TestValues()
        {
            Assert.AreEqual("123", 123.ToJson());
            Assert.AreEqual("true", true.ToJson());
            Assert.AreEqual("false", false.ToJson());
            Assert.AreEqual("[1,2,3]", new int[] { 1, 2, 3 }.ToJson());
            Assert.AreEqual("[1,2,3]", new List<int> { 1, 2, 3 }.ToJson());
        }

        [Test]
        public void TestDicts()
        {
            Assert.AreEqual("{\"foo\":\"bar\"}", new Dictionary<string, string> { { "foo", "bar" } }.ToJson());
            Assert.AreEqual("{\"foo\":123}", new Dictionary<string, int> { { "foo", 123 } }.ToJson());
        }

        class SimpleObject
        {
            public SimpleObject A;
            public List<int> B;
            public string C { get; set; }
        }

        struct SimpleStruct
        {
            public SimpleObject A;
        }

        [Test]
        public void TestObjects()
        {
            Assert.AreEqual("{\"A\":{},\"B\":[1,2,3],\"C\":\"Test\"}", new SimpleObject { A = new SimpleObject(), B = new List<int> { 1, 2, 3 }, C = "Test" }.ToJson());
            Assert.AreEqual("{\"A\":{\"A\":{},\"B\":[1,2,3],\"C\":\"Test\"}}", new SimpleStruct { A = new SimpleObject { A = new SimpleObject(), B = new List<int> { 1, 2, 3 }, C = "Test" } }.ToJson());
        }
    }
}
