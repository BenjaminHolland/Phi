using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phi.Interop.WinAPI.Files
{
    public class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            [In] string lpFileName,
            [In][MarshalAs(UnmanagedType.U4)] NativeAccessMode dwDesiredAccess,
            [In][MarshalAs(UnmanagedType.U4)] NativeShareMode dwShareMode,
            [In][Optional] IntPtr lpSecurityAttributes,
            [In][MarshalAs(UnmanagedType.U4)] NativeCreateMode dwCreationDisposition,
            [In][MarshalAs(UnmanagedType.U4)] NativeFileAttributes dwFlagsAndAttributes,
            [In][Optional] IntPtr hTemplateFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFileEx(
            [In] IntPtr hFile,
            [Out][Optional] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToRead,
            [In][Out] IntPtr lpOverlapped,
            [In] IOCompletionCallback lpCompletionRoutine);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFileEx(
            [In] IntPtr hFile,
            [Out][Optional] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToWrite,
            [In][Out] IntPtr lpOverlapped,
            [In] IOCompletionCallback lpCompletionRoutine);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            [In] IntPtr hFile,
            [Out] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToRead,
            [Out][Optional] out uint lpNumberOfBytesRead,
            [In][Out][Optional] IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(
            [In] IntPtr hFile,
            [In] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToWrite,
            [Out][Optional] out uint lpNumberOfBytesWritten,
            [In][Out][Optional] IntPtr lpOverlapped);
    }
}
