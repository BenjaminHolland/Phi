using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Phi.WinAPI
{
    [Flags]
    public enum NativeDeviceIOMethod : uint
    {
        Buffered = 0,
        InDirect = 1,
        OutDirect = 2,
        Neither = 3
    }
    [Flags]
    public enum NativeDeviceIODevice : uint
    {
        Beep = 0x00000001,
        CDRom = 0x00000002,
        CDRomFileSytem = 0x00000003,
        Controller = 0x00000004,
        Datalink = 0x00000005,
        Dfs = 0x00000006,
        Disk = 0x00000007,
        DiskFileSystem = 0x00000008,
        FileSystem = 0x00000009,
        InPortPort = 0x0000000a,
        Keyboard = 0x0000000b,
        Mailslot = 0x0000000c,
        MidiIn = 0x0000000d,
        MidiOut = 0x0000000e,
        Mouse = 0x0000000f,
        MultiUncProvider = 0x00000010,
        NamedPipe = 0x00000011,
        Network = 0x00000012,
        NetworkBrowser = 0x00000013,
        NetworkFileSystem = 0x00000014,
        Null = 0x00000015,
        ParallelPort = 0x00000016,
        PhysicalNetcard = 0x00000017,
        Printer = 0x00000018,
        Scanner = 0x00000019,
        SerialMousePort = 0x0000001a,
        SerialPort = 0x0000001b,
        Screen = 0x0000001c,
        Sound = 0x0000001d,
        Streams = 0x0000001e,
        Tape = 0x0000001f,
        TapeFileSystem = 0x00000020,
        Transport = 0x00000021,
        Unknown = 0x00000022,
        Video = 0x00000023,
        VirtualDisk = 0x00000024,
        WaveIn = 0x00000025,
        WaveOut = 0x00000026,
        Port8042 = 0x00000027,
        NetworkRedirector = 0x00000028,
        Battery = 0x00000029,
        BusExtender = 0x0000002a,
        Modem = 0x0000002b,
        Vdm = 0x0000002c,
        MassStorage = 0x0000002d,
        Smb = 0x0000002e,
        Ks = 0x0000002f,
        Changer = 0x00000030,
        Smartcard = 0x00000031,
        Acpi = 0x00000032,
        Dvd = 0x00000033,
        FullscreenVideo = 0x00000034,
        DfsFileSystem = 0x00000035,
        DfsVolume = 0x00000036,
        Serenum = 0x00000037,
        Termsrv = 0x00000038,
        Ksec = 0x00000039,
        // From Windows Driver Kit 7
        Fips = 0x0000003A,
        Infiniband = 0x0000003B,
        Vmbus = 0x0000003E,
        CryptProvider = 0x0000003F,
        Wpd = 0x00000040,
        Bluetooth = 0x00000041,
        MtComposite = 0x00000042,
        MtTransport = 0x00000043,
        Biometric = 0x00000044,
        Pmi = 0x00000045
    }

    /// <summary>
    /// IO Control Codes
    /// Useful links:
    ///     http://www.ioctls.net/
    ///     http://msdn.microsoft.com/en-us/library/windows/hardware/ff543023(v=vs.85).aspx
    /// </summary>
    [Flags]
    public enum NativeDeviceIOControlCode : uint
    {
        // STORAGE
        StorageCheckVerify = (NativeDeviceIODevice.MassStorage << 16) | (0x0200 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageCheckVerify2 = (NativeDeviceIODevice.MassStorage << 16) | (0x0200 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14), // FileAccess.Any
        StorageMediaRemoval = (NativeDeviceIODevice.MassStorage << 16) | (0x0201 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageEjectMedia = (NativeDeviceIODevice.MassStorage << 16) | (0x0202 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageLoadMedia = (NativeDeviceIODevice.MassStorage << 16) | (0x0203 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageLoadMedia2 = (NativeDeviceIODevice.MassStorage << 16) | (0x0203 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        StorageReserve = (NativeDeviceIODevice.MassStorage << 16) | (0x0204 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageRelease = (NativeDeviceIODevice.MassStorage << 16) | (0x0205 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageFindNewDevices = (NativeDeviceIODevice.MassStorage << 16) | (0x0206 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageEjectionControl = (NativeDeviceIODevice.MassStorage << 16) | (0x0250 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        StorageMcnControl = (NativeDeviceIODevice.MassStorage << 16) | (0x0251 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        StorageGetMediaTypes = (NativeDeviceIODevice.MassStorage << 16) | (0x0300 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        StorageGetMediaTypesEx = (NativeDeviceIODevice.MassStorage << 16) | (0x0301 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        StorageResetBus = (NativeDeviceIODevice.MassStorage << 16) | (0x0400 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageResetDevice = (NativeDeviceIODevice.MassStorage << 16) | (0x0401 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        StorageGetDeviceNumber = (NativeDeviceIODevice.MassStorage << 16) | (0x0420 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        StoragePredictFailure = (NativeDeviceIODevice.MassStorage << 16) | (0x0440 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        StorageObsoleteResetBus = (NativeDeviceIODevice.MassStorage << 16) | (0x0400 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        StorageObsoleteResetDevice = (NativeDeviceIODevice.MassStorage << 16) | (0x0401 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        StorageQueryProperty = (NativeDeviceIODevice.MassStorage << 16) | (0x0500 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        // DISK
        DiskGetDriveGeometry = (NativeDeviceIODevice.Disk << 16) | (0x0000 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskGetDriveGeometryEx = (NativeDeviceIODevice.Disk << 16) | (0x0028 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskGetPartitionInfo = (NativeDeviceIODevice.Disk << 16) | (0x0001 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskGetPartitionInfoEx = (NativeDeviceIODevice.Disk << 16) | (0x0012 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskSetPartitionInfo = (NativeDeviceIODevice.Disk << 16) | (0x0002 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGetDriveLayout = (NativeDeviceIODevice.Disk << 16) | (0x0003 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskSetDriveLayout = (NativeDeviceIODevice.Disk << 16) | (0x0004 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskVerify = (NativeDeviceIODevice.Disk << 16) | (0x0005 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskFormatTracks = (NativeDeviceIODevice.Disk << 16) | (0x0006 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskReassignBlocks = (NativeDeviceIODevice.Disk << 16) | (0x0007 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskPerformance = (NativeDeviceIODevice.Disk << 16) | (0x0008 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskIsWritable = (NativeDeviceIODevice.Disk << 16) | (0x0009 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskLogging = (NativeDeviceIODevice.Disk << 16) | (0x000a << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskFormatTracksEx = (NativeDeviceIODevice.Disk << 16) | (0x000b << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskHistogramStructure = (NativeDeviceIODevice.Disk << 16) | (0x000c << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskHistogramData = (NativeDeviceIODevice.Disk << 16) | (0x000d << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskHistogramReset = (NativeDeviceIODevice.Disk << 16) | (0x000e << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskRequestStructure = (NativeDeviceIODevice.Disk << 16) | (0x000f << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskRequestData = (NativeDeviceIODevice.Disk << 16) | (0x0010 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskControllerNumber = (NativeDeviceIODevice.Disk << 16) | (0x0011 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskSmartGetVersion = (NativeDeviceIODevice.Disk << 16) | (0x0020 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskSmartSendDriveCommand = (NativeDeviceIODevice.Disk << 16) | (0x0021 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskSmartRcvDriveData = (NativeDeviceIODevice.Disk << 16) | (0x0022 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskUpdateDriveSize = (NativeDeviceIODevice.Disk << 16) | (0x0032 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGrowPartition = (NativeDeviceIODevice.Disk << 16) | (0x0034 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGetCacheInformation = (NativeDeviceIODevice.Disk << 16) | (0x0035 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskSetCacheInformation = (NativeDeviceIODevice.Disk << 16) | (0x0036 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskDeleteDriveLayout = (NativeDeviceIODevice.Disk << 16) | (0x0040 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskFormatDrive = (NativeDeviceIODevice.Disk << 16) | (0x00f3 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskSenseDevice = (NativeDeviceIODevice.Disk << 16) | (0x00f8 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskCheckVerify = (NativeDeviceIODevice.Disk << 16) | (0x0200 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskMediaRemoval = (NativeDeviceIODevice.Disk << 16) | (0x0201 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskEjectMedia = (NativeDeviceIODevice.Disk << 16) | (0x0202 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskLoadMedia = (NativeDeviceIODevice.Disk << 16) | (0x0203 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskReserve = (NativeDeviceIODevice.Disk << 16) | (0x0204 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskRelease = (NativeDeviceIODevice.Disk << 16) | (0x0205 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskFindNewDevices = (NativeDeviceIODevice.Disk << 16) | (0x0206 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        DiskGetMediaTypes = (NativeDeviceIODevice.Disk << 16) | (0x0300 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskSetPartitionInfoEx = (NativeDeviceIODevice.Disk << 16) | (0x0013 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGetDriveLayoutEx = (NativeDeviceIODevice.Disk << 16) | (0x0014 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        DiskSetDriveLayoutEx = (NativeDeviceIODevice.Disk << 16) | (0x0015 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskCreateDisk = (NativeDeviceIODevice.Disk << 16) | (0x0016 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGetLengthInfo = (NativeDeviceIODevice.Disk << 16) | (0x0017 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        // CHANGER
        ChangerGetParameters = (NativeDeviceIODevice.Changer << 16) | (0x0000 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerGetStatus = (NativeDeviceIODevice.Changer << 16) | (0x0001 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerGetProductData = (NativeDeviceIODevice.Changer << 16) | (0x0002 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerSetAccess = (NativeDeviceIODevice.Changer << 16) | (0x0004 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        ChangerGetElementStatus = (NativeDeviceIODevice.Changer << 16) | (0x0005 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        ChangerInitializeElementStatus = (NativeDeviceIODevice.Changer << 16) | (0x0006 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerSetPosition = (NativeDeviceIODevice.Changer << 16) | (0x0007 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerExchangeMedium = (NativeDeviceIODevice.Changer << 16) | (0x0008 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerMoveMedium = (NativeDeviceIODevice.Changer << 16) | (0x0009 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerReinitializeTarget = (NativeDeviceIODevice.Changer << 16) | (0x000A << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        ChangerQueryVolumeTags = (NativeDeviceIODevice.Changer << 16) | (0x000B << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        // FILESYSTEM
        FsctlRequestOplockLevel1 = (NativeDeviceIODevice.FileSystem << 16) | (0 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlRequestOplockLevel2 = (NativeDeviceIODevice.FileSystem << 16) | (1 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlRequestBatchOplock = (NativeDeviceIODevice.FileSystem << 16) | (2 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlOplockBreakAcknowledge = (NativeDeviceIODevice.FileSystem << 16) | (3 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlOpBatchAckClosePending = (NativeDeviceIODevice.FileSystem << 16) | (4 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlOplockBreakNotify = (NativeDeviceIODevice.FileSystem << 16) | (5 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlLockVolume = (NativeDeviceIODevice.FileSystem << 16) | (6 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlUnlockVolume = (NativeDeviceIODevice.FileSystem << 16) | (7 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlDismountVolume = (NativeDeviceIODevice.FileSystem << 16) | (8 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlIsVolumeMounted = (NativeDeviceIODevice.FileSystem << 16) | (10 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlIsPathnameValid = (NativeDeviceIODevice.FileSystem << 16) | (11 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlMarkVolumeDirty = (NativeDeviceIODevice.FileSystem << 16) | (12 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlQueryRetrievalPointers = (NativeDeviceIODevice.FileSystem << 16) | (14 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlGetCompression = (NativeDeviceIODevice.FileSystem << 16) | (15 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlSetCompression = (NativeDeviceIODevice.FileSystem << 16) | (16 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        FsctlMarkAsSystemHive = (NativeDeviceIODevice.FileSystem << 16) | (19 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlOplockBreakAckNo2 = (NativeDeviceIODevice.FileSystem << 16) | (20 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlInvalidateVolumes = (NativeDeviceIODevice.FileSystem << 16) | (21 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlQueryFatBpb = (NativeDeviceIODevice.FileSystem << 16) | (22 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlRequestFilterOplock = (NativeDeviceIODevice.FileSystem << 16) | (23 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlFileSystemGetStatistics = (NativeDeviceIODevice.FileSystem << 16) | (24 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlGetNtfsVolumeData = (NativeDeviceIODevice.FileSystem << 16) | (25 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlGetNtfsFileRecord = (NativeDeviceIODevice.FileSystem << 16) | (26 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlGetVolumeBitmap = (NativeDeviceIODevice.FileSystem << 16) | (27 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlGetRetrievalPointers = (NativeDeviceIODevice.FileSystem << 16) | (28 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlMoveFile = (NativeDeviceIODevice.FileSystem << 16) | (29 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlIsVolumeDirty = (NativeDeviceIODevice.FileSystem << 16) | (30 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlGetHfsInformation = (NativeDeviceIODevice.FileSystem << 16) | (31 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlAllowExtendedDasdIo = (NativeDeviceIODevice.FileSystem << 16) | (32 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlReadPropertyData = (NativeDeviceIODevice.FileSystem << 16) | (33 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlWritePropertyData = (NativeDeviceIODevice.FileSystem << 16) | (34 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlFindFilesBySid = (NativeDeviceIODevice.FileSystem << 16) | (35 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlDumpPropertyData = (NativeDeviceIODevice.FileSystem << 16) | (37 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlSetObjectId = (NativeDeviceIODevice.FileSystem << 16) | (38 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlGetObjectId = (NativeDeviceIODevice.FileSystem << 16) | (39 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlDeleteObjectId = (NativeDeviceIODevice.FileSystem << 16) | (40 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlSetReparsePoint = (NativeDeviceIODevice.FileSystem << 16) | (41 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlGetReparsePoint = (NativeDeviceIODevice.FileSystem << 16) | (42 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlDeleteReparsePoint = (NativeDeviceIODevice.FileSystem << 16) | (43 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlEnumUsnData = (NativeDeviceIODevice.FileSystem << 16) | (44 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlSecurityIdCheck = (NativeDeviceIODevice.FileSystem << 16) | (45 << 2) | NativeDeviceIOMethod.Neither | (FileAccess.Read << 14),
        FsctlReadUsnJournal = (NativeDeviceIODevice.FileSystem << 16) | (46 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlSetObjectIdExtended = (NativeDeviceIODevice.FileSystem << 16) | (47 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlCreateOrGetObjectId = (NativeDeviceIODevice.FileSystem << 16) | (48 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlSetSparse = (NativeDeviceIODevice.FileSystem << 16) | (49 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlSetZeroData = (NativeDeviceIODevice.FileSystem << 16) | (50 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Write << 14),
        FsctlQueryAllocatedRanges = (NativeDeviceIODevice.FileSystem << 16) | (51 << 2) | NativeDeviceIOMethod.Neither | (FileAccess.Read << 14),
        FsctlEnableUpgrade = (NativeDeviceIODevice.FileSystem << 16) | (52 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Write << 14),
        FsctlSetEncryption = (NativeDeviceIODevice.FileSystem << 16) | (53 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlEncryptionFsctlIo = (NativeDeviceIODevice.FileSystem << 16) | (54 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlWriteRawEncrypted = (NativeDeviceIODevice.FileSystem << 16) | (55 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlReadRawEncrypted = (NativeDeviceIODevice.FileSystem << 16) | (56 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlCreateUsnJournal = (NativeDeviceIODevice.FileSystem << 16) | (57 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlReadFileUsnData = (NativeDeviceIODevice.FileSystem << 16) | (58 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlWriteUsnCloseRecord = (NativeDeviceIODevice.FileSystem << 16) | (59 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlExtendVolume = (NativeDeviceIODevice.FileSystem << 16) | (60 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlQueryUsnJournal = (NativeDeviceIODevice.FileSystem << 16) | (61 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlDeleteUsnJournal = (NativeDeviceIODevice.FileSystem << 16) | (62 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlMarkHandle = (NativeDeviceIODevice.FileSystem << 16) | (63 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlSisCopyFile = (NativeDeviceIODevice.FileSystem << 16) | (64 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        FsctlSisLinkFiles = (NativeDeviceIODevice.FileSystem << 16) | (65 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        FsctlHsmMsg = (NativeDeviceIODevice.FileSystem << 16) | (66 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.ReadWrite << 14),
        FsctlNssControl = (NativeDeviceIODevice.FileSystem << 16) | (67 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Write << 14),
        FsctlHsmData = (NativeDeviceIODevice.FileSystem << 16) | (68 << 2) | NativeDeviceIOMethod.Neither | (FileAccess.ReadWrite << 14),
        FsctlRecallFile = (NativeDeviceIODevice.FileSystem << 16) | (69 << 2) | NativeDeviceIOMethod.Neither | (0 << 14),
        FsctlNssRcontrol = (NativeDeviceIODevice.FileSystem << 16) | (70 << 2) | NativeDeviceIOMethod.Buffered | (FileAccess.Read << 14),
        // VIDEO
        VideoQuerySupportedBrightness = (NativeDeviceIODevice.Video << 16) | (0x0125 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        VideoQueryDisplayBrightness = (NativeDeviceIODevice.Video << 16) | (0x0126 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14),
        VideoSetDisplayBrightness = (NativeDeviceIODevice.Video << 16) | (0x0127 << 2) | NativeDeviceIOMethod.Buffered | (0 << 14)
    }
    [Flags]
    public enum NativeAccessMode : uint
    {
        #region Generic Rights
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,
        #endregion
        #region Standard Rights
        StandardDelete = 0x10000,
        StandardReadControl = 0x20000,
        StandardWriteDAC = 0x40000,
        StandardWriteOwner = 0x80000,
        StandardSynchronize = 0x100000,

        StandardRightsRead = StandardReadControl,
        StandardRightsWrite = StandardReadControl,
        StandardRightsExecute = StandardReadControl,
        #endregion
        #region Attribute Rights
        ReadAttributes = 0x0080,
        WriteAttributes = 0x0100,
        #endregion
        #region File Rights
        FileRead = 0x0001,
        FileWrite = 0x0002,
        FileAppend = 0x0004,
        FileReadAttributesEx = 0x0008,
        FileWriteAttributesEx = 0x0010,
        FileExecute = 0x0020,
        #endregion
        #region Directory Rights
        DirectoryListDirectory = FileRead,
        DirectoryAddFile = FileAppend,
        DirectoryAddSubdirectory = FileAppend,
        DirectoryReadAttributesEx = FileReadAttributesEx,
        DirectoryWriteAttributesEx = FileWriteAttributesEx,
        DirectoryTraverse = 0x0020,
        DirectoryDeleteChild = 0x0040,
        #endregion
        #region Pipe Rights
        PipeRead = FileRead,
        PipeWrite = FileWrite,
        PipeCreateInstance = FileAppend,
        #endregion
    }
    [Flags]
    public enum NativeShareMode : uint
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4,
    }
    public enum NativeCreateMode : uint
    {
        New = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5,
    }
    [Flags]
    public enum NativeFileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        FirstPipeInstance = 0x00080000
    }

    public static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true,EntryPoint =@"DeviceIoControl")]
        public static extern bool DeviceIOControl(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U4)] NativeDeviceIOControlCode controlCode,
            IntPtr pInBuffer,
            uint sizeInBuffer,
            IntPtr pOutBuffer,
            uint sizeOutBuffer,
            ref uint bytesReturned,
            IntPtr overlapped
            );
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPTStr)] string fileName,
            [MarshalAs(UnmanagedType.U4)] NativeAccessMode accessMode,
            [MarshalAs(UnmanagedType.U4)] NativeShareMode shareMode,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] NativeCreateMode createMode,
            [MarshalAs(UnmanagedType.U4)] NativeFileAttributes attributes,
            IntPtr templateFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFileEx(
            IntPtr handle,
            IntPtr pBuffer,
            uint bytesToRead,
            IntPtr pNativeOverlapped,
            IOCompletionCallback callback);
        [DllImport("kernel32.dll",SetLastError =true)]
        public static extern bool ReadFile(
            IntPtr handle,
            IntPtr pBuffer,
            uint bytesToRead,
            ref uint pBytesRead,
            IntPtr pNativeOverlapped
            );
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFileEx(
            IntPtr handle,
            IntPtr pBuffer,
            uint bytesToWrite,
            IntPtr pNativeOverlapped,
            IOCompletionCallback callback);
    }
}
