using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phi.IO.Devices;
using Phi.Core.Interop;
namespace Phi.Test {
    [TestClass]
    public class TestNativeDrive {
        [TestMethod]
        [TestCategory("Phi.IO.Devices.NativeDrive")]
        public void ReadTest() {
            var drive = new NativeDrive("\\\\.\\PHYSICALDRIVE1");
            GlobalBuffer buffer = new GlobalBuffer(1024, 1024 * 4);
            var result = drive.BeginRead(0, buffer.DangerousGetHandle(), 1024);
            int read = drive.EndRead(result);
            Console.WriteLine($"Read {read} bytes");
            byte[] mBuffer = new byte[1024];
            buffer.ReadArray(0, mBuffer, 0, 1024);
            int i = 0;
            while (i < 1024) {
                for(int j = 0; j < 8; j++) {
                    Console.Write(mBuffer[i+j].ToString().PadLeft(5));
                }
                Console.WriteLine();
                i += 8;
            }
            Assert.IsTrue(read > 0);
            drive.Dispose();
        }
    }
}
