using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace Phi.WinAPI {
    public class AlignedHGlobalBuffer:SafeBuffer {
        private IntPtr _raw;
        [Conditional("DEBUG")]
        private void _checkAlignment(int alignment) {
            if(((long)handle)%alignment!=0){
                throw new InvalidOperationException("alignment failed");
            }
        }
        public AlignedHGlobalBuffer(int size, int alignment):base(true) {
            if (alignment % 8 != 0) {
                throw new ArgumentException("alignment must be %8");
            }
            _raw = Marshal.AllocHGlobal(size + alignment);
            IntPtr alignedPtr = new IntPtr(alignment * (((long)_raw + alignment - 1) / alignment));
            
            SetHandle(alignedPtr);
            Initialize((ulong)size);
            _checkAlignment(alignment);
        }
        protected override bool ReleaseHandle() {
            Marshal.FreeHGlobal(_raw);
            SetHandleAsInvalid();
            return true;
        }
    }
}
