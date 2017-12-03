using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyJson;

namespace TinyJson.Test
{
    [TestClass]
    public class TestWriter
    {
        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual("123", 123.ToJson());
            Assert.AreEqual("true", true.ToJson());
            Assert.AreEqual("false", false.ToJson());
            Assert.AreEqual("[1,2,3]", new int[] { 1, 2, 3 }.ToJson());
            Assert.AreEqual("[1,2,3]", new List<int> { 1, 2, 3 }.ToJson());
        }

        [TestMethod]
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

        [TestMethod]
        public void TestObjects()
        {
            Assert.AreEqual("{\"A\":{},\"B\":[1,2,3],\"C\":\"Test\"}", new SimpleObject { A = new SimpleObject(), B = new List<int> { 1, 2, 3 }, C = "Test" }.ToJson());
            Assert.AreEqual("{\"A\":{\"A\":{},\"B\":[1,2,3],\"C\":\"Test\"}}", new SimpleStruct { A = new SimpleObject { A = new SimpleObject(), B = new List<int> { 1, 2, 3 }, C = "Test" } }.ToJson());
        }

        public struct NastyStruct
        {
            public int R, G, B;
            public NastyStruct(byte r, byte g, byte b)
            {
                R = r; G = g; B = b;
            }
            public static NastyStruct Nasty = new NastyStruct(0, 0, 0);
        }

        [TestMethod]
        public void TestNastyStruct()
        {
            Assert.AreEqual("{\"R\":1,\"G\":2,\"B\":3}", new NastyStruct(1,2,3).ToJson());
        }

        [TestMethod]
        public void TestEscaping()
        {
            Assert.AreEqual("{\"hello\":\"world\\n \\\\ \\\" \\b \\r \\0\"}", new Dictionary<string,string>{
                {"hello", "world\n \\ \" \b \r \0"}
            }.ToJson());
        }
    }
}
