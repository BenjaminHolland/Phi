using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Interop.WinAPI.IO
{
    public class NativeMethods
    {
        [DllImport("kernel32.dll",SetLastError =true)]
        public static extern bool DeviceIoControl(
           [In] IntPtr hDevice,
           [In][MarshalAs(UnmanagedType.U4)] NativeDeviceIOControlCode dwIoControlCode,
           [In][Optional] IntPtr lpInBuffer,
           [In] uint nInBufferSize,
           [Out][Optional] IntPtr lpOutBuffer,
           [In] uint nOutBufferSize,
           [Out][Optional] out uint lpBytesReturned,
           [In][Out][Optional] IntPtr lpOverlapped);
    }
}
