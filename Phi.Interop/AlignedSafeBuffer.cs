using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Interop
{
    public abstract class AlignedSafeBuffer : SafeBuffer
    {
        private readonly Action<IntPtr> _free;
        private IntPtr _raw;
        public AlignedSafeBuffer(int size, int alignment, Func<int, IntPtr> alloc, Action<IntPtr> free) : base(true)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            if (alignment < 0) throw new ArgumentOutOfRangeException(nameof(alignment));
            if (alloc == null) throw new ArgumentNullException(nameof(alloc));
            if (free == null) throw new ArgumentNullException(nameof(free));
            if (alignment == 0)
            {

                _raw = alloc(size);
                SetHandle(_raw);
            }
            else
            {
                _raw = alloc(size + alignment);
                IntPtr alignedPtr = new IntPtr(alignment * (((long)_raw + alignment - 1) / alignment));
                SetHandle(alignedPtr);
            }
            _free = free;
            Initialize((ulong)size);
        }
        protected override bool ReleaseHandle()
        {
            _free(_raw);
            SetHandleAsInvalid();
            return true;
        }
    }
}
