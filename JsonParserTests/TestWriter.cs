using JsonParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JsonParserTests
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
    }
}
