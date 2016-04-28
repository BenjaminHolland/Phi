using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace Phi.IO
{
    [StructLayout(LayoutKind.Sequential,Size =30)]
    public struct NativeDiskGeometryEx
    {
        public ulong Cylinders;
        public byte MediaType;
        public uint TracksPerCylinder;
        public uint SectorsPerTrack;
        public uint BytesPerSector;
        public ulong DiskSize;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =1)]
        public byte[] Data;
    }
}
