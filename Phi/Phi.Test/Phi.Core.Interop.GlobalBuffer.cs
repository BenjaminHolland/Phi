using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phi.Core.Interop;
namespace Phi.Core.Interop.Test {
    [TestClass]
    public class TestAlignedHGlobalBuffer {
        [TestMethod]
        [TestCategory("Phi.Core.Interop.GlobalBuffer")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeSizeTest() {
            GlobalBuffer buffer = new GlobalBuffer(-1, 8);
        }
        [TestMethod]
        [TestCategory("Phi.Core.Interop.GlobalBuffer")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeAlignmentTest() {
            GlobalBuffer buffer = new GlobalBuffer(128, -1);
        }
        [TestMethod]
        [TestCategory("Phi.Core.Interop.GlobalBuffer")]
        public void RepeatDisposalTest() {
            GlobalBuffer buffer = new GlobalBuffer(255, 8);
            buffer.Dispose();
            buffer.Dispose();
        }
        [TestMethod]
        [TestCategory("Phi.Core.Interop.GlobalBuffer")]
        public void AlignmentTest() {
            GlobalBuffer buffer = new GlobalBuffer(1, 13);
            Assert.AreEqual(0, buffer.DangerousGetHandle().ToInt32() % 13);
        }
    }
}
