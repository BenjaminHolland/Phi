using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace Phi.Core.Interop {
    public sealed class GlobalBuffer : SafeBuffer {
        private IntPtr _raw;
        public GlobalBuffer(int size)
            : this(size, 0) {
        }
        public GlobalBuffer(int size, int alignment) : base(true) {
            if (size <= 0) throw new ArgumentOutOfRangeException("size");
            if (alignment < 0) throw new ArgumentOutOfRangeException("alignment");
            if (alignment == 0) {
                _raw = Marshal.AllocHGlobal(size);
                SetHandle(_raw);
            }
            else {
                _raw = Marshal.AllocHGlobal(size + alignment);
                IntPtr alignedPtr = new IntPtr(alignment * (((long)_raw + alignment - 1) / alignment));
                SetHandle(alignedPtr);
            }
            Initialize((ulong)size);
        }
        protected override bool ReleaseHandle() {
            Marshal.FreeHGlobal(_raw);
            SetHandleAsInvalid();
            return true;
        }
    }
}
