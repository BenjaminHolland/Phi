using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phi.Interop.Win32;
namespace Phi.Test {
    [TestClass]
    public class AlignedNativeBufferTests {
        [TestMethod]
        [TestCategory("Phi.Interop.Win32.AlignedNativeBuffer")]
        public void MassAlignmentTest() {
            for (int i = 0; i < 10000000; i++) {
                AlignedNativeBuffer buffer = new AlignedNativeBuffer(514, 4096);
                buffer.Dispose();
            }
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [TestMethod]
        [TestCategory("Phi.Interop.Win32.PageCache")]
        public void PageCacheCreation() {
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            PageCache cache = new PageCache(PageCache.TestHandle, 8, 512, 1);
            cache.AccessBlock(0, System.IO.FileAccess.Write).Write(0,(ushort)0xFFFF);
            cache.AccessBlock(1);
            Assert.AreEqual((ushort)0xFFFF, cache.AccessBlock(0).ReadUInt16(0));
            cache.AccessBlock(2);
            PageCache.Shutdown();
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [TestMethod]
        [TestCategory("Phi.Interop.Win32.PageCache")]
        public void PageCacheThreadedTest() {
            PageCache cache = new PageCache(PageCache.TestHandle, 8, 512, 1);
            cache.AccessBlock(0, System.IO.FileAccess.Write).Write(0, (ushort)0xFFFF);      
            System.Threading.Tasks.Parallel.For(0,2,(i)=>{
                cache.AccessBlock((ulong)i, System.IO.FileAccess.ReadWrite).Write(0, (ushort)0xFFFF);
            });
            Assert.AreEqual((ushort)0xFFFF,cache.AccessBlock)
            PageCache.Shutdown();
        }
    }
    
}
