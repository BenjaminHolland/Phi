using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phi.Core;
using Phi.IO.Parsing;
namespace Phi.Test {
    [TestClass]
    public class BitConverterExTests {
        [TestMethod]
        [TestCategory("Phi.Core.BitConverterEx")]
        public void ToUInt32() {
            Assert.AreEqual((UInt32)0xFEFFFFFF,BitConverterEx.ToUInt32(HexParser.ToByteArray("FFFFFFFE"), 0, ByteOrder.BigEndian));
            Assert.AreEqual((UInt32)0xFFFFFFFE, BitConverterEx.ToUInt32(HexParser.ToByteArray("FFFFFFFE"), 0, ByteOrder.LittleEndian));
        }
        [TestMethod]
        [TestCategory("Phi.Core.BitConverterEx")]
        public void ToUInt24() {
            Assert.AreEqual((UInt32)0x00FEFFFF, BitConverterEx.ToUInt24(HexParser.ToByteArray("FFFFFE"), 0, ByteOrder.BigEndian));
            Assert.AreEqual((UInt32)0x00FFFFFE,BitConverterEx.ToUInt24(HexParser.ToByteArray("FFFFFE"),0,ByteOrder.LittleEndian));
        }
        [TestMethod]
        [TestCategory("Phi.Core.BitConverterEx")]
        public void ToUInt16() {
            Assert.AreEqual((UInt16)0xFEFF, BitConverterEx.ToUInt16(HexParser.ToByteArray("FFFE"), 0, ByteOrder.BigEndian));
            Assert.AreEqual((UInt16)0xFFFE, BitConverterEx.ToUInt16(HexParser.ToByteArray("FFFE"), 0, ByteOrder.LittleEndian));
        }
        [TestMethod]
        [TestCategory("Phi.Core.BitConverterEx")]
        public void ToInt32() {
            Assert.AreEqual(unchecked((Int32)0xFEFFFFFF), BitConverterEx.ToInt32(HexParser.ToByteArray("FFFFFFFE"), 0, ByteOrder.BigEndian));
            Assert.AreEqual(unchecked((Int32)0xFFFFFFFE), BitConverterEx.ToInt32(HexParser.ToByteArray("FFFFFFFE"), 0, ByteOrder.LittleEndian));
        }
        [TestMethod]
        [TestCategory("Phi.Core.BitConverterEx")]
        public void ToInt24() {
            Assert.AreEqual(unchecked((Int32)0xFFFEFFFF), BitConverterEx.ToInt24(HexParser.ToByteArray("FFFFFE"), 0, ByteOrder.BigEndian));
            Assert.AreEqual(unchecked((Int32)0xFFFFFFFE), BitConverterEx.ToInt24(HexParser.ToByteArray("FFFFFE"), 0, ByteOrder.LittleEndian));
        }
        [TestMethod]
        [TestCategory("Phi.Core.BitConverterEx")]
        public void ToInt16() {
            Assert.AreEqual(unchecked((Int16)0xFEFF), BitConverterEx.ToInt16(HexParser.ToByteArray("FFFE"), 0, ByteOrder.BigEndian));
            Assert.AreEqual(unchecked((Int16)0xFFFE), BitConverterEx.ToInt16(HexParser.ToByteArray("FFFE"), 0, ByteOrder.LittleEndian));
        }
    }
}
