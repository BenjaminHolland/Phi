using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Phi.Win32
{

    public static class Kernel32
    {
        internal const string ImportModuleName = "kernel32.dll";
        [DllImport(ImportModuleName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            [In] string lpFileName,
            [In][MarshalAs(UnmanagedType.U4)] NativeAccessMode dwDesiredAccess,
            [In][MarshalAs(UnmanagedType.U4)] NativeShareMode dwShareMode,
            [In][Optional] IntPtr lpSecurityAttributes,
            [In][MarshalAs(UnmanagedType.U4)] NativeCreateMode dwCreationDisposition,
            [In][MarshalAs(UnmanagedType.U4)] NativeFileAttributes dwFlagsAndAttributes,
            [In][Optional] IntPtr hTemplateFile);
        [DllImport(ImportModuleName, SetLastError = true)]
        public static extern bool ReadFileEx(
            [In] IntPtr hFile,
            [Out][Optional] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToRead,
            [In][Out] IntPtr lpOverlapped,
            [In] IOCompletionCallback lpCompletionRoutine);
        [DllImport(ImportModuleName, SetLastError = true)]
        public static extern bool WriteFileEx(
            [In] IntPtr hFile,
            [Out][Optional] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToWrite,
            [In][Out] IntPtr lpOverlapped,
            [In] IOCompletionCallback lpCompletionRoutine);
        [DllImport(ImportModuleName, SetLastError = true)]
        public static extern bool ReadFile(
            [In] IntPtr hFile,
            [Out] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToRead,
            [Out][Optional] out uint lpNumberOfBytesRead,
            [In][Out][Optional] IntPtr lpOverlapped);
        [DllImport(ImportModuleName, SetLastError = true)]
        public static extern bool WriteFile(
            [In] IntPtr hFile,
            [In] IntPtr lpBuffer,
            [In] uint nNumberOfBytesToWrite,
            [Out][Optional] out uint lpNumberOfBytesWritten,
            [In][Out][Optional] IntPtr lpOverlapped);
        [DllImport(ImportModuleName, SetLastError = true)]
        public static extern bool CloseHandle(
            [In] IntPtr hObject);
        [DllImport(ImportModuleName, SetLastError = true)]
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
