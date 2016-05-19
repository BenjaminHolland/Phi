using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Interop.WinAPI.Handle
{
    public class NativeMethods
    {
        [DllImport("kernel32.dll",SetLastError =true)]
        public static extern bool CloseHandle(
            [In] IntPtr hObject);
    }
}
