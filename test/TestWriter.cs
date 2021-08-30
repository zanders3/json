using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyJson;

namespace TinyJson.Test
{
    [TestClass]
    public class TestWriter
    {
        public enum Color
        {
            Red,
            Green,
            Blue,
            Yellow
        }

        [Flags]
        public enum Style
        {
            None = 0,
            Bold = 1,
            Italic = 2,
            Underline = 4,
            Strikethrough = 8
        }

        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual(new DateTime().ToJson(), "01/01/0001 00:00:00");
            Assert.AreEqual("\"\u94b1\u4e0d\u591f!\"", "\u94b1\u4e0d\u591f!".ToJson());
            Assert.AreEqual("123", 123.ToJson());
            Assert.AreEqual("true", true.ToJson());
            Assert.AreEqual("false", false.ToJson());
            Assert.AreEqual("[1,2,3]", new int[] { 1, 2, 3 }.ToJson());
            Assert.AreEqual("[1,2,3]", new List<int> { 1, 2, 3 }.ToJson());
            Assert.AreEqual("\"Green\"", Color.Green.ToJson());
            Assert.AreEqual("\"Green\"", ((Color)1).ToJson());
            Assert.AreEqual("\"10\"", ((Color)10).ToJson());
            Assert.AreEqual("\"Bold\"", Style.Bold.ToJson());
            Assert.AreEqual("\"Bold, Italic\"", (Style.Bold | Style.Italic).ToJson());
            Assert.AreEqual("\"19\"", (Style.Bold | Style.Italic | (Style)16).ToJson());
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

            // Should not serialize
            private int D = 333;
            public static int E = 555;
            internal int F = 777;
            protected int G = 999;
            public const int H = 111;
        }

        struct SimpleStruct
        {
            public SimpleObject A;
        }

        class InheritedObject : SimpleObject
        {
            public int X;
        }

        [TestMethod]
        public void TestObjects()
        {
            Assert.AreEqual("{\"A\":{},\"B\":[1,2,3],\"C\":\"Test\"}", new SimpleObject { A = new SimpleObject(), B = new List<int> { 1, 2, 3 }, C = "Test" }.ToJson());
            Assert.AreEqual("{\"A\":{\"A\":{},\"B\":[1,2,3],\"C\":\"Test\"}}", new SimpleStruct { A = new SimpleObject { A = new SimpleObject(), B = new List<int> { 1, 2, 3 }, C = "Test" } }.ToJson());
            Assert.AreEqual("{\"X\":9,\"A\":{},\"B\":[1,2,3],\"C\":\"Test\"}", new InheritedObject { A = new SimpleObject(), B = new List<int> { 1, 2, 3 }, C = "Test", X = 9 }.ToJson());
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
            Assert.AreEqual("{\"hello\":\"world\\n \\\\ \\\" \\b \\r \\u0000\u263A\"}", new Dictionary<string,string>{
                {"hello", "world\n \\ \" \b \r \0\u263A"}
            }.ToJson());
        }

        class IgnoreDataMemberObject
        {
            [IgnoreDataMember]
            public int A;
            public int B;
            [IgnoreDataMember]
            public int C { get; set; }
            public int D { get; set; }
        }

        [TestMethod]
        public void TestIgnoreDataMemberObject()
        {
            Assert.AreEqual("{\"B\":20,\"D\":40}", new IgnoreDataMemberObject { A = 10, B = 20, C = 30, D = 40 }.ToJson());
        }

        class DataMemberObject
        {
            [DataMember(Name = "a")]
            public int A;
            [DataMember()]
            public int B;
            [DataMember(Name = "c")]
            public int C { get; set; }
            public int D { get; set; }
        }

        [TestMethod]
        public void TestDataMemberObject()
        {
            Assert.AreEqual("{\"a\":10,\"B\":20,\"c\":30,\"D\":40}", new DataMemberObject { A = 10, B = 20, C = 30, D = 40 }.ToJson());
        }

        public class EnumClass
        {
            public Color Colors;
            public Style Style;
        }

        [TestMethod]
        public void TestEnumMember()
        {
            Assert.AreEqual("{\"Colors\":\"Green\",\"Style\":\"Bold\"}", new EnumClass { Colors = Color.Green, Style = Style.Bold }.ToJson());
            Assert.AreEqual("{\"Colors\":\"Green\",\"Style\":\"Bold, Underline\"}", new EnumClass { Colors = Color.Green, Style = Style.Bold | Style.Underline }.ToJson());
            Assert.AreEqual("{\"Colors\":\"Blue\",\"Style\":\"Italic, Underline\"}", new EnumClass { Colors = (Color)2, Style = (Style)6 }.ToJson());
            Assert.AreEqual("{\"Colors\":\"Blue\",\"Style\":\"Underline\"}", new EnumClass { Colors = (Color)2, Style = (Style)4 }.ToJson());
            Assert.AreEqual("{\"Colors\":\"10\",\"Style\":\"17\"}", new EnumClass { Colors = (Color)10, Style = (Style)17 }.ToJson());
        }

        public class PrimitiveObject
        {
            public bool Bool;
            public byte Byte;
            public sbyte SByte;
            public short Short;
            public ushort UShort;
            public int Int;
            public uint UInt;
            public long Long;
            public ulong ULong;
            public char Char;
            public float Single;
            public double Double;
            public decimal Decimal;
            public DateTime Time;
        }

        [TestMethod]
        public void TestPrimitives()
        {
            Assert.AreEqual(
                "{\"Bool\":true,\"Byte\":17,\"SByte\":-17,\"Short\":-123,\"UShort\":123,\"Int\":-56,\"UInt\":56,\"Long\":-34,\"ULong\":34,\"Char\":\"C\",\"Single\":4.3,\"Double\":5.6,\"Decimal\":10.1,\"Time\":\"08/16/2021 00:00:00\"}",
                new PrimitiveObject
                {
                    Bool = true,
                    Byte = 17,
                    SByte = -17,
                    Short = -123,
                    UShort = 123,
                    Int = -56,
                    UInt = 56,
                    Long = -34,
                    ULong = 34,
                    Char = 'C',
                    Single = 4.3f,
                    Double = 5.6,
                    Decimal = 10.1M,
                    Time = new DateTime(2021, 8, 16)
                }.ToJson());
        }

        [TestMethod]
        public void TestDatetime()
        {
            DateTime curTime =  DateTime.Now;
            Assert.AreEqual(curTime.ToJson(), "\"" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\"");
        }

    }
}
