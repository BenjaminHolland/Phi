using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phi.WinAPI;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Phi.IO.Devices {
    public class NativeDrive : IDisposable {
        private bool _isDisposed;
        public void Dispose() {
            if (!_isDisposed) {

                //Console.WriteLine("Disposing");
                _isDisposed = true;
                //Kernel32.CloseHandle(_handle.DangerousGetHandle());
                _handle.Dispose();
            }
        }
        unsafe static void DriveTransferComplete(uint errorCode, uint bytesRead, NativeOverlapped* npOverlapped) {
            bool unpacked = false;
            try {

                //Console.WriteLine($"Drive transfer callback called.\nError Code: {errorCode}\nBytes Transferred: {bytesRead}");
                Overlapped mOverlapped = Overlapped.Unpack(npOverlapped);
                unpacked = true;
                DriveTransferResult result = mOverlapped.AsyncResult as DriveTransferResult;
                result.SetComplete((int)bytesRead);
            }
            finally {
                if (!unpacked) {
                    Overlapped.Unpack(npOverlapped);
                }
                Overlapped.Free(npOverlapped);
            }
        }
        class DriveTransferResult : IAsyncResult {

            private EventWaitHandle _waitHandle;
            public DriveTransferResult() {
                _waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            }
            public int BytesTransferred {
                get;
                private set;
            }
            public object AsyncState {
                get {
                    return null;
                }
            }

            public WaitHandle AsyncWaitHandle {
                get {
                    return _waitHandle;
                }

            }
            public void SetComplete(int bytesTransferred) {
                if (!IsCompleted) {
                    IsCompleted = true;
                    BytesTransferred = bytesTransferred;
                    _waitHandle.Set();
                }
            }
            public void WaitForCompletion() {
                if (!IsCompleted) {
                    _waitHandle.WaitOne();
                    _waitHandle.Dispose();
                    _waitHandle = null;
                }
            }
            public bool CompletedSynchronously {
                get;
                private set;
            }

            public bool IsCompleted {
                get;
                private set;
            }
            ~DriveTransferResult() {
                if (!IsCompleted) {
                    if (_waitHandle != null) {
                        _waitHandle.Dispose();
                    }
                }
            }
        }

        SafeFileHandle _handle;
        public NativeDrive(string name) {
            _handle = new SafeFileHandle(
                Kernel32.CreateFile(
                    name,
                    NativeAccessMode.GenericRead | NativeAccessMode.GenericWrite,
                    NativeShareMode.Read | NativeShareMode.Write,
                    IntPtr.Zero,
                    NativeCreateMode.OpenExisting,
                    NativeFileAttributes.Overlapped | NativeFileAttributes.RandomAccess | NativeFileAttributes.NoBuffering,
                    IntPtr.Zero),
                true);

            int errorCode = 0;
            uint transferred = 0;
            if (_handle.IsInvalid) {
                throw new Win32Exception();
            }
            else {
                if (!Kernel32.DeviceIOControl(_handle.DangerousGetHandle(), NativeDeviceIOControlCode.FsctlLockVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, ref transferred, IntPtr.Zero)) {
                    errorCode = Marshal.GetLastWin32Error();

                    //Kernel32.CloseHandle(_handle.DangerousGetHandle());
                    _handle.Dispose();
                    throw new Win32Exception(errorCode);
                }
                else {
                    if (!Kernel32.DeviceIOControl(_handle.DangerousGetHandle(), NativeDeviceIOControlCode.FsctlDismountVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, ref transferred, IntPtr.Zero)) {
                        errorCode = Marshal.GetLastWin32Error();
                        if (!Kernel32.DeviceIOControl(_handle.DangerousGetHandle(), NativeDeviceIOControlCode.FsctlUnlockVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, ref transferred, IntPtr.Zero)) {

                            //Kernel32.CloseHandle(_handle.DangerousGetHandle());
                            _handle.Dispose();
                            throw new Win32Exception("", new Win32Exception(errorCode));
                        }
                        else {
                            //Kernel32.CloseHandle(_handle.DangerousGetHandle());
                            _handle.Dispose();
                            throw new Win32Exception(errorCode);
                        }
                    }
                }
            }
            //if (!ThreadPool.BindHandle(_handle))
            //{
            //    _handle.Dispose();
            //    throw new Win32Exception();
            //}
        }

        public IAsyncResult BeginWrite(ulong bytePos, IntPtr pBuffer, uint count) {
            Overlapped mOverlapped = new Overlapped();
            mOverlapped.AsyncResult = new DriveTransferResult();
            mOverlapped.OffsetHigh = (int)((bytePos >> 16) >> 16);
            mOverlapped.OffsetLow = (int)(bytePos & 0x00000000FFFFFFFF);

            unsafe
            {
                NativeOverlapped* upOverlapped = mOverlapped.Pack(null, null);
                IntPtr mpOverlapped = new IntPtr(upOverlapped);
                if (!Kernel32.WriteFileEx(_handle.DangerousGetHandle(), pBuffer, count, mpOverlapped, DriveTransferComplete)) {
                    Overlapped.Unpack(upOverlapped);
                    Overlapped.Free(upOverlapped);
                    throw new Win32Exception();
                }
                else {
                    return mOverlapped.AsyncResult;
                }
            }
        }
        public int EndWrite(IAsyncResult asyncResult) {
            DriveTransferResult result = asyncResult as DriveTransferResult;
            if (result == null) {
                throw new ArgumentException("Bad Result");
            }
            result.WaitForCompletion();
            return result.BytesTransferred;
        }

        public Task<int> ReadAsync(ulong bytePos,IntPtr pBuffer,uint count) {
            IAsyncResult token= BeginRead(bytePos, pBuffer, count);
            return Task.Run(() => {
                return EndRead(token);
            });
        }
        public IAsyncResult BeginRead(ulong bytePos, IntPtr pBuffer, uint count) {
            Overlapped mOverlapped = new Overlapped();
            mOverlapped.AsyncResult = new DriveTransferResult();
            mOverlapped.OffsetHigh = (int)((bytePos >> 16) >> 16);
            mOverlapped.OffsetLow = (int)(bytePos & 0x00000000FFFFFFFF);

            unsafe
            {
                NativeOverlapped* upOverlapped = mOverlapped.Pack(null, null);
                IntPtr mpOverlapped = new IntPtr(upOverlapped);
                if (!Kernel32.ReadFileEx(_handle.DangerousGetHandle(), pBuffer, count, mpOverlapped, DriveTransferComplete)) {
                    Overlapped.Unpack(upOverlapped);
                    Overlapped.Free(upOverlapped);
                    throw new Win32Exception();
                }
                else {
                    return mOverlapped.AsyncResult;
                }
            }
        }
        public int EndRead(IAsyncResult asyncResult) {
            DriveTransferResult result = asyncResult as DriveTransferResult;
            if (result == null) {
                throw new ArgumentException("Bad Result");
            }
            result.WaitForCompletion();
            return result.BytesTransferred;
        }
    }
}
