using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace Phi.Interop.Win32 {
    public class AlignedNativeBuffer:SafeBuffer {
        private IntPtr _raw;
        
        [Conditional("DEBUG")]
        private void _checkAlignment(int alignment) {
            if(((long)this.handle)%alignment!=0){
                throw new InvalidOperationException("alignment failed");
            }
        }
        public AlignedNativeBuffer(Int32 size, Int32 alignment):base(true) {
            if (alignment % 8 != 0) {
                throw new ArgumentException("alignment must be %8");
            }
            _raw = Marshal.AllocHGlobal(size + alignment);
            IntPtr alignedPtr = new IntPtr(alignment * (((long)_raw + alignment - 1) / alignment));
            
            this.SetHandle(alignedPtr);
            this.Initialize((ulong)size);
            _checkAlignment(alignment);
        }
        protected override bool ReleaseHandle() {
            Marshal.FreeHGlobal(_raw);
            this.SetHandleAsInvalid();
            return true;
        }
    }
}
