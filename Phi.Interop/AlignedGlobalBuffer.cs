-using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace Phi.Interop {
    public sealed class AlignedGlobalBuffer : AlignedSafeBuffer
    {
        public AlignedGlobalBuffer(int size,int alignment) : base(size, alignment, Marshal.AllocHGlobal, Marshal.FreeHGlobal) { }
        public AlignedGlobalBuffer(int size) : this(size, 0) { }
    }
}
