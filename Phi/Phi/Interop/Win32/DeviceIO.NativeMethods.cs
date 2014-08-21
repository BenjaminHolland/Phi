using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
namespace Phi.Interop.Win32.DeviceIO {
    /// <summary>
    /// FileSystem Control Codes
    /// </summary>
    public static class FSCTLCodes {
        public static readonly CTLCode FSCTL_LOCK_VOLUME = new CTLCode(Device.FileSystem, 6, Method.Buffered, Access.Any);
        public static readonly CTLCode FSCTL_UNLOCK_VOLUME = new CTLCode(Device.FileSystem, 7, Method.Buffered, Access.Any);
        public static readonly CTLCode FSCTL_DISMOUNT_VOLUME = new CTLCode(Device.FileSystem, 8, Method.Buffered, Access.Any);
    }
    /// <summary>
    /// Control Code Structure for use with DeviceIOControl
    /// </summary>
    public struct CTLCode {
        private uint _value;
        private CTLCode(uint value) {
            _value = value;
        }
        public CTLCode(Device deviceType, uint function, Method method, Access fileAccess) {
            _value = (((uint)deviceType << 16) | ((uint)fileAccess << 14) | (function << 2) | (uint)method);
        }
        public Method CodeMethod {
            get {
                return (Method)((_value & 0xffff0000) >> 16);
            }
        }
        public Device CodeDevice {
            get {
                return (Device)(_value & 3);
            }
        }
        public uint Value {
            get {
                return _value;
            }
        }
        public static implicit operator uint(CTLCode code) {
            return code.Value;
        }
        public static implicit operator CTLCode(uint code) {
            return new CTLCode(code);
        }
    }
    [Flags]
    public enum Access : uint {
        Any = 0,
        Special = 0,
        Read = 0x0001,
        Write = 0x0002,
    }
    public enum Method : uint {
        Buffered = 0,
        DirectIn = 1,
        DirectOut = 2,
        Neither = 3,

    }
    public enum Device : uint {
        Beep = 0x00000001,
        CDROM,
        CDROMFileSystem,
        Controller,
        DataLink,
        DFS,
        Disk,
        DiskFileSystem,
        FileSystem,
        InPortPort,
        Keyboard,
        MailSlot,
        MIDIIn,
        MIDIOut,
        Mouse,
        MultiUNCProvider,
        NamedPipe,
        Network,
        NetworkBrowser,
        NetworkFileSystem,
        Null,
        ParallelPort,
        PhysicalNetCard,
        Printer,
        Scanner,
        SerialMousePort,
        Screen,
        Sound,
        Streams,
        Tape,
        TapeFileSystem,
        Transport,
        Unknown,
        Video,
        VirtualDisk,
        WaveIn,
        WaveOut,
        Port8042,
        NetworkRedirector,
        Battery,
        BusExtender,
        Modem,
        VDM,
        MassStorage,
        SMB,
        KS,
        Changer,
        SmartCard,
        ACPI,
        DVD,
        FullscreenVideo,
        DFSFileSystem,
        DFSVolume,
        SereNum,
        TermSrv,
        KSEC,
        FIPS,
        InfinityBand,
        VMBus,
        CryptProvider,
        WPD,
        BlueTooth,
        MTComposite,
        MTTransport,
        Biometric,
        PMI,
        EHSTOR,
        DevAPI,
        GPIO,
        USBEX,
        Console,
        NFP,
        SysEnv,
        VirtualBlock,
        PointOfService,
    }
    internal class NativeMethods {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("Kernel32.dll")]
        public static unsafe extern Boolean DeviceIoControl(
                                                  IntPtr hDevice,
                                                  UInt32 dwIoControlCode,
                                                  IntPtr lpInBuffer,
                                                  UInt32 nInBufferSize,
                                                  IntPtr lpOutBuffer,
                                                  UInt32 nOutBufferSize,
                                                  out UInt32 lpBytesReturned,
                                                  NativeOverlapped* lpOverlapped);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("Kernel32.dll")]
        public static unsafe extern Boolean DeviceIoControl(
                                                  IntPtr hDevice,
                                                  UInt32 dwIoControlCode,
                                                  SafeBuffer lpInBuffer,
                                                  UInt32 nInBufferSize,
                                                  SafeBuffer lpOutBuffer,
                                                  UInt32 nOutBufferSize,
                                                  out UInt32 lpBytesReturned,
                                                  NativeOverlapped* lpOverlapped);
    }
}
