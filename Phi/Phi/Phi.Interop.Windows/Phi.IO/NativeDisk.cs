using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Phi.Win32;
using System.Threading;
using System.Runtime.InteropServices;
using System.Management;
namespace Phi.IO
{
    internal class Win32_ObjectWrapper : IDisposable
    {
        public string DeviceID
        {
            get
            {
                return _source["DeviceID"] as string;
            }
        }
        public ManagementPath Path
        {
            get
            {
                return _source.Path;
            }
        }
        protected readonly ManagementObject _source;
        public Win32_ObjectWrapper(ManagementObject source)
        {
            _source = source;
        }
        public Win32_ObjectWrapper(ManagementPath path)
        {
            _source = new ManagementObject(path);
            _source.Get();
        }
        public void Dispose()
        {
            _source.Dispose();
        }
    }
    internal sealed class Win32_LogicalDisk : Win32_ObjectWrapper
    {
        public Win32_LogicalDisk(ManagementObject source) : base(source) { }
        public IEnumerable<Win32_DiskPartition> Partitions
        {
            get
            {
                using (ManagementObjectCollection moc = _source.GetRelated("Win32_DiskPartition"))
                {
                    foreach (ManagementObject mo in moc)
                    {
                        yield return new Win32_DiskPartition(mo);
                    }
                }
            }
        }
    }
    internal sealed class Win32_DiskPartition : Win32_ObjectWrapper
    {

        public Win32_DiskPartition(ManagementObject source) : base(source) { }
        public IEnumerable<Win32_LogicalDisk> LogicalDisks
        {
            get
            {
                using (ManagementObjectCollection moc = _source.GetRelated("Win32_LogicalDisk"))
                {
                    foreach (ManagementObject mo in moc)
                    {
                        yield return new Win32_LogicalDisk(mo);
                    }
                }
            }
        }
    }
    internal sealed class Win32_DiskDrive : Win32_ObjectWrapper
    {
        public Win32_DiskDrive(ManagementObject source) : base(source)
        {

        }
        public IEnumerable<Win32_DiskPartition> Partitions
        {
            get
            {
                using (ManagementObjectCollection moc = _source.GetRelated("Win32_DiskPartition"))
                {
                    foreach (ManagementObject mo in moc)
                    {
                        yield return new Win32_DiskPartition(mo);
                    }
                }
            }
        }

    }
    public sealed class NativeDriveRecord
    {
        public string DrivePath
        {
            get;
            private set;
        }
        private readonly List<string> _volumes;
        public IEnumerable<string> Volumes
        {
            get
            {
                return _volumes.AsEnumerable();
            }
        }
        public NativeDriveRecord(string path, IEnumerable<string> volumes)
        {
            DrivePath = path;
            _volumes = new List<string>(volumes.Select(s => s == null ? "?:" : s));
        }
        public override string ToString()
        {
            return $"{DrivePath} ({string.Join(",", Volumes)})";
        }
    }
    public class NativeDrive : IDisposable
    {
        public static IEnumerable<NativeDriveRecord> Drives
        {
            get
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery("SELECT * FROM Win32_DiskDrive")))
                {
                    using (ManagementObjectCollection moc = searcher.Get())
                    {
                        foreach (ManagementObject mo in moc)
                        {
                            Win32_DiskDrive drive = new Win32_DiskDrive(mo);
                            List<string> volumeNames = new List<string>();
                            foreach (Win32_DiskPartition partition in drive.Partitions)
                            {
                                int counter = 0;
                                foreach (Win32_LogicalDisk disk in partition.LogicalDisks)
                                {
                                    counter++;
                                    volumeNames.Add(disk.DeviceID);
                                    disk.Dispose();
                                }
                                if (counter == 0)
                                {
                                    volumeNames.Add(null);
                                }
                                partition.Dispose();
                            }
                            yield return new NativeDriveRecord(drive.DeviceID, volumeNames);
                            drive.Dispose();
                        }

                    }
                }
            }
        }

        private unsafe static NativeOverlapped* PackForControl(TaskCompletionSource<object> tcs)
        {
            var mOl = new Overlapped(0, 0, IntPtr.Zero, null);
            var nOl = mOl.Pack((errorCode, bytesTransferred, nOlLocal) =>
            {
                try
                {
                    switch (errorCode)
                    {
                        case ErrorCodes.ERROR_SUCCESS:
                            tcs.TrySetResult(null);
                            break;
                        case ErrorCodes.ERROR_OPERATION_ABORTED:
                            tcs.TrySetCanceled();
                            break;
                        default:
                            tcs.TrySetException(new Win32Exception((int)errorCode));
                            break;
                    }
                }
                finally
                {
                    Overlapped.Unpack(nOlLocal);
                    Overlapped.Free(nOlLocal);
                }
            }, null);
            return nOl;
        }
        private unsafe static Task DispatchControl(bool completedSyncronously, NativeOverlapped* nOl, TaskCompletionSource<object> tcs)
        {
            if (completedSyncronously)
            {
                Overlapped.Unpack(nOl);
                Overlapped.Free(nOl);
                tcs.SetResult(null);
                return tcs.Task;
            }
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode == ErrorCodes.ERROR_IO_PENDING)
            {
                return tcs.Task;
            }
            Overlapped.Unpack(nOl);
            Overlapped.Free(nOl);
            throw new Win32Exception(errorCode);
        }

        private static unsafe NativeOverlapped* PackForTransfer(long pos, TaskCompletionSource<uint> tcs)
        {
            var mOl = new Overlapped((int)(pos & 0xffffffff), (int)(pos >> 32), IntPtr.Zero, null);
            var nOl = mOl.Pack((errorCode, bytesTransferred, nOlLocal) =>
            {
                try
                {
                    switch (errorCode)
                    {
                        case ErrorCodes.ERROR_SUCCESS:

                            tcs.TrySetResult(bytesTransferred);
                            break;
                        case ErrorCodes.ERROR_OPERATION_ABORTED:
                            tcs.TrySetCanceled();
                            break;
                        default:
                            tcs.TrySetException(new Win32Exception((int)errorCode));
                            break;
                    }
                }
                finally
                {
                    Overlapped.Unpack(nOlLocal);
                    Overlapped.Free(nOlLocal);
                }
            }, null);
            return nOl;
        }
        private unsafe static Task<uint> DispatchTransfer(uint t, bool completedSyncronously, NativeOverlapped* nOl, TaskCompletionSource<uint> tcs)
        {
            if (completedSyncronously)
            {
                Overlapped.Unpack(nOl);
                Overlapped.Free(nOl);
                tcs.SetResult(t);
                return tcs.Task;
            }
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode == ErrorCodes.ERROR_IO_PENDING)
            {
                return tcs.Task;
            }
            Overlapped.Unpack(nOl);
            Overlapped.Free(nOl);
            throw new Win32Exception(errorCode);
        }


        private readonly SafeFileHandle _handle;
        private int _disposeCount;
        public NativeDrive(string path)
        {
            _handle = new SafeFileHandle(Kernel32.CreateFile(path, NativeAccessMode.GenericRead | NativeAccessMode.GenericWrite, NativeShareMode.Read | NativeShareMode.Write, IntPtr.Zero, NativeCreateMode.OpenExisting, NativeFileAttributes.Overlapped | NativeFileAttributes.NoBuffering, IntPtr.Zero), true);
            if (_handle.IsInvalid) throw new Win32Exception();
            ThreadPool.BindHandle(_handle);
        }

        public NativeDiskGeometryEx GetGeometry()
        {
            return GetGeometryAsync().GetAwaiter().GetResult();
        }
        public Task<NativeDiskGeometryEx> GetGeometryAsync()
        {

            byte[] buffer = new byte[Marshal.SizeOf<NativeDiskGeometryEx>()];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var tcs = new TaskCompletionSource<NativeDiskGeometryEx>();
            var mOl = new Overlapped(0, 0, IntPtr.Zero, null);
            unsafe
            {
                var nOl = mOl.Pack((error, transferred, nOlLocal) =>
                {

                    try
                    {
                        switch (error)
                        {
                            case ErrorCodes.ERROR_SUCCESS:
                                tcs.TrySetResult(Marshal.PtrToStructure<NativeDiskGeometryEx>(handle.AddrOfPinnedObject()));
                                break;
                            case ErrorCodes.ERROR_OPERATION_ABORTED:
                                tcs.TrySetCanceled();
                                break;
                            default:
                                tcs.TrySetException(new Win32Exception((int)error));
                                break;
                        }

                    }
                    finally
                    {

                        Overlapped.Unpack(nOlLocal);
                        Overlapped.Free(nOlLocal);
                        handle.Free();
                    }
                }, handle);
                uint read = 0;
                bool complete = Kernel32.DeviceIoControl(_handle.DangerousGetHandle(), NativeDeviceIOControlCode.DiskGetDriveGeometryEx, IntPtr.Zero, 0, handle.AddrOfPinnedObject(), (uint)buffer.Length, out read, (IntPtr)nOl);

                if (complete)
                {
                    Overlapped.Unpack(nOl);
                    Overlapped.Free(nOl);
                    tcs.SetResult(Marshal.PtrToStructure<NativeDiskGeometryEx>(handle.AddrOfPinnedObject()));
                    handle.Free();

                    return tcs.Task;
                }
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode == ErrorCodes.ERROR_IO_PENDING)
                {
                    return tcs.Task;
                }
                Overlapped.Unpack(nOl);
                Overlapped.Free(nOl);
                handle.Free();
                throw new Win32Exception(errorCode);
            }
        }
        public uint Read(long pos, uint count, byte[] buffer)
        {
            return ReadAsync(pos, count, buffer).GetAwaiter().GetResult();
        }
        public uint Read(long pos, uint count, IntPtr buffer)
        {
            return ReadAsync(pos, count, buffer).GetAwaiter().GetResult();
        }
        public uint Write(long pos, uint count, byte[] buffer)
        {
            return WriteAsync(pos, count, buffer).GetAwaiter().GetResult();
        }
        public uint Write(long pos, uint count, IntPtr buffer)
        {
            return WriteAsync(pos, count, buffer).GetAwaiter().GetResult();
        }
        public void Lock()
        {
            LockAsync().GetAwaiter().GetResult();
        }
        public void Unlock()
        {
            UnlockAsync().GetAwaiter().GetResult();
        }

        public Task<uint> ReadAsync(long pos, uint count, byte[] buffer)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var tcs = new TaskCompletionSource<uint>();
            var mOl = new Overlapped((int)(pos & 0xffffffff), (int)(pos >> 32), IntPtr.Zero, null);
            unsafe
            {
                var nOl = mOl.Pack((error, transferred, nOlLocal) =>
                  {

                      try
                      {
                          switch (error)
                          {
                              case ErrorCodes.ERROR_SUCCESS:
                                  tcs.TrySetResult(transferred);
                                  break;
                              case ErrorCodes.ERROR_OPERATION_ABORTED:
                                  tcs.TrySetCanceled();
                                  break;
                              default:
                                  tcs.TrySetException(new Win32Exception((int)error));
                                  break;
                          }

                      }
                      finally
                      {

                          Overlapped.Unpack(nOlLocal);
                          Overlapped.Free(nOlLocal);
                          handle.Free();
                      }
                  }, handle);
                uint read = 0;
                bool complete = Kernel32.ReadFile(_handle.DangerousGetHandle(), handle.AddrOfPinnedObject(), count, out read, (IntPtr)nOl);

                if (complete)
                {
                    Overlapped.Unpack(nOl);
                    Overlapped.Free(nOl);
                    handle.Free();
                    tcs.SetResult(read);
                    return tcs.Task;
                }
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode == ErrorCodes.ERROR_IO_PENDING)
                {
                    return tcs.Task;
                }
                Overlapped.Unpack(nOl);
                Overlapped.Free(nOl);
                handle.Free();
                throw new Win32Exception(errorCode);
            }
        }
        public Task<uint> ReadAsync(long pos, uint count, IntPtr buffer)
        {
            var tcs = new TaskCompletionSource<uint>();
            unsafe
            {
                var nOl = PackForTransfer(pos, tcs);
                uint read = 0;
                bool complete = Kernel32.ReadFile(_handle.DangerousGetHandle(), buffer, count, out read, (IntPtr)nOl);
                return DispatchTransfer(read, complete, nOl, tcs);
            }
        }
        public Task<uint> WriteAsync(long pos, uint count, byte[] buffer)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var tcs = new TaskCompletionSource<uint>();
            var mOl = new Overlapped((int)(pos & 0xffffffff), (int)(pos >> 32), IntPtr.Zero, null);
            unsafe
            {
                var nOl = mOl.Pack((error, transferred, nOlLocal) =>
                {

                    try
                    {
                        switch (error)
                        {
                            case ErrorCodes.ERROR_SUCCESS:
                                tcs.TrySetResult(transferred);
                                break;
                            case ErrorCodes.ERROR_OPERATION_ABORTED:
                                tcs.TrySetCanceled();
                                break;
                            default:
                                tcs.TrySetException(new Win32Exception((int)error));
                                break;
                        }

                    }
                    finally
                    {

                        Overlapped.Unpack(nOlLocal);
                        Overlapped.Free(nOlLocal);
                        handle.Free();
                    }
                }, handle);
                uint read = 0;
                bool complete = Kernel32.WriteFile(_handle.DangerousGetHandle(), handle.AddrOfPinnedObject(), count, out read, (IntPtr)nOl);

                if (complete)
                {
                    Overlapped.Unpack(nOl);
                    Overlapped.Free(nOl);
                    handle.Free();
                    tcs.SetResult(read);
                    return tcs.Task;
                }
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode == ErrorCodes.ERROR_IO_PENDING)
                {
                    return tcs.Task;
                }
                Overlapped.Unpack(nOl);
                Overlapped.Free(nOl);
                handle.Free();
                throw new Win32Exception(errorCode);
            }
        }
        public Task<uint> WriteAsync(long pos, uint count, IntPtr buffer)
        {
            var tcs = new TaskCompletionSource<uint>();
            unsafe
            {
                var nOl = PackForTransfer(pos, tcs);
                uint read = 0;
                bool complete = Kernel32.WriteFile(_handle.DangerousGetHandle(), buffer, count, out read, (IntPtr)nOl);
                return DispatchTransfer(read, complete, nOl, tcs);
            }
        }
        public Task LockAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            unsafe
            {
                var nOl = PackForControl(tcs);
                uint transferred = 0;
                bool complete = Kernel32.DeviceIoControl(_handle.DangerousGetHandle(), NativeDeviceIOControlCode.FsctlLockVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, out transferred, (IntPtr)nOl);
                return DispatchControl(complete, nOl, tcs);
            }
        }
        public Task UnlockAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            unsafe
            {
                var nOl = PackForControl(tcs);
                uint transferred = 0;
                bool complete = Kernel32.DeviceIoControl(_handle.DangerousGetHandle(), NativeDeviceIOControlCode.FsctlUnlockVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, out transferred, (IntPtr)nOl);
                return DispatchControl(complete, nOl, tcs);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Increment(ref _disposeCount) > 1) return;
            try
            {
                UnlockAsync().GetAwaiter().GetResult();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ErrorCodes.ERROR_NOT_LOCKED) { }
                else throw ex;
            }
            _handle.Dispose();

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~NativeDrive()
        {
            Dispose(false);
        }
    }
}
