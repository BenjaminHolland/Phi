using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phi.IO.Parsing;
namespace Phi.Test {
    [TestClass]
    public class HexParserTests {
        [TestMethod]
        [TestCategory("Phi.IO.Parsing.HexParser")]
        public void HexParserTest1() {
            Assert.AreEqual(255,HexParser.ToByteArray("FF")[0]);
            Assert.AreEqual(15, HexParser.ToByteArray("0F")[0]);
            Assert.AreEqual(16, HexParser.ToByteArray("10")[0]);

            foreach (char c in ":;<=>?@") {
                bool causedOverflow = false;
                try {
                    HexParser.ToByteArray("0:");
                    causedOverflow = true;
                }
                catch (ArgumentException ex) {
                    Assert.AreEqual("hexString",ex.Message);
                    Assert.IsFalse(causedOverflow);
                }
            }
            var NegativeTwoBytes = HexParser.ToByteArray("FFFE");
            Assert.AreEqual(255, NegativeTwoBytes[0]);
            Assert.AreEqual(254, NegativeTwoBytes[1]);
            
        }
    }
}
