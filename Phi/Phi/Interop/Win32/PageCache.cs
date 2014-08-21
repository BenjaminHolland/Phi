using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO;
using Phi.Core;
using Phi.Core.Development;
namespace Phi.Interop.Win32 {
    [Flags]
    public enum PageBufferErrorStates {
        None = 0,
        ReadFailure = 0x1,
        WriteFailure = 0x2,
        OpenFailure = 0x4,
    }
    /// <summary>
    /// Class used to buffer and manipulate data provided by native handles. This class does not own the handle. 
    /// </summary>
    [TODO("Document class")]
    public class PageCache {
#if DEBUG
        public static readonly SafeFileHandle TestHandle = _createTestHandle();
        
            private static SafeFileHandle _createTestHandle(){
                SafeFileHandle ret=FileIO.NativeMethods.CreateFile("PageCacheTestMemory", Phi.Interop.Win32.FileIO.FileAccess.GenericRead|Phi.Interop.Win32.FileIO.FileAccess.GenericWrite, FileIO.FileShare.Read | FileIO.FileShare.Write, default(IntPtr), FileIO.CreationDisposition.CreateAlways, FileIO.FileAttributes.Normal, IntPtr.Zero);
                AlignedNativeBuffer buffer=new AlignedNativeBuffer(1024,8);
                UnmanagedMemoryStream test=new UnmanagedMemoryStream(buffer,0,1024,FileAccess.ReadWrite);
                for(int i=0;i<1024;i++){
                    test.WriteByte(0);
                }
                test.Dispose();
                uint retBytes=0;
                long retPtr = 0;
                unsafe {
                    FileIO.NativeMethods.WriteFile(ret, buffer, 1024,out retBytes, null);
                    FileIO.NativeMethods.SeekFile(ret, 0, out retPtr, FileIO.FileSeekMethod.FileBegin);
                }
                return ret;
            }
        public static void Shutdown() {
            TestHandle.Dispose();
        }
#endif
        public PageBufferErrorStates ErrorState {
            get;
            protected set;
        }
        public Int32 LastErrorCode {
            get;
            protected set;
        }
        private AlignedNativeBuffer _page;
        private UInt32 _pageSize;
        private UInt32 _blockSize;
        private UInt32 _pageIdx;
        private Boolean _needsFlush;
        private SafeFileHandle _handle;
        /// <summary>
        /// The index of the first sector currently stored in the Page Buffer.
        /// </summary>
        private UInt64 _firstSectorInPage {
            get {
                return ((_pageIdx * _pageSize) / _blockSize);
            }
        }
        /// <summary>
        /// The index of the last sector currently stored in the Page Buffer.
        /// </summary>
        private UInt64 _lastSectorInPage {
            get {
                return (((_pageIdx + 1) * _pageSize) / _blockSize) - 1;
            }
        }
        /// <summary>
        /// Whether the given sector is buffered or not.
        /// </summary>
        /// <param name="sectorIdx">the index of the target sector.</param>
        /// <returns>true if the sector is in the buffered page, false if it is not.</returns>
        private Boolean _isBlockInPage(UInt64 blockIdx) {
            if (blockIdx < _firstSectorInPage || blockIdx > _lastSectorInPage)
                return false;
            return true;
        }
        /// <summary>
        /// Get the index of the page that contains the given sector.
        /// </summary>
        /// <param name="sectorIdx">The index of the target sector</param>
        /// <returns>The index of the page that contains the given sector.</returns>
        private UInt32 _indexOfPageContaining(UInt64 blockIdx) {
            return (UInt32)((blockIdx * _blockSize) / _pageSize);
        }
        /// <summary>
        /// Find the byte index of the first byte of the given sector relative to the start of the buffer.
        /// </summary>
        /// <param name="sectorIdx">The sector to locate</param>
        /// <returns>The byte index of the first byte of the given sector relative to the start of the buffer, or -1 if the sector is not currently in the buffer.</returns>
        private Int32 _firstByteInBlock(UInt64 blockIdx) {
            if (!_isBlockInPage(blockIdx)) {
                return -1;
            }
            ulong firstBlockInPage = (((_pageIdx) * _pageSize) / _blockSize);
            ulong blockIdxInPage = (uint)(blockIdx - firstBlockInPage);
            return (Int32)(blockIdxInPage * _blockSize);
        }
        private unsafe bool _writeTo(UInt32 pageIdx) {
            bool success = false;
            long retPtr;
            uint retBytes = 0;
            if(!FileIO.NativeMethods.SeekFile(_handle,pageIdx*_pageSize,out retPtr,FileIO.FileSeekMethod.FileBegin)){
                LastErrorCode=Marshal.GetLastWin32Error();
                ErrorState=PageBufferErrorStates.WriteFailure;
            }else{
                if (!FileIO.NativeMethods.WriteFile(_handle, _page, _pageSize, out retBytes, null)) {
                    LastErrorCode = Marshal.GetLastWin32Error();
                    ErrorState = PageBufferErrorStates.WriteFailure;
                }
                else {
                    if (retBytes == _pageSize) {
                        success = true;
                    }
                    else {
                        ErrorState = PageBufferErrorStates.WriteFailure;
                    }
                }
            }
            return success;
        }
        private unsafe bool _readFrom(UInt32 pageIdx) {
            bool success = false;
            long retPtr;
            uint retBytes=0;
            if(!FileIO.NativeMethods.SeekFile(_handle,pageIdx*_pageSize,out retPtr,FileIO.FileSeekMethod.FileBegin)){
                LastErrorCode=Marshal.GetLastWin32Error();
                ErrorState=PageBufferErrorStates.ReadFailure;
            }else{
                _pageIdx=pageIdx;
                if(!FileIO.NativeMethods.ReadFile(_handle,_page,_pageSize,out retBytes,null)){
                    LastErrorCode=Marshal.GetLastWin32Error();
                    ErrorState=PageBufferErrorStates.ReadFailure;
                }else{
                    if(retBytes==_pageSize){
                        success=true;
                    }else{
                        ErrorState=PageBufferErrorStates.ReadFailure;
                    }
                }
            }
            return success;
        }        
        private void _throwIfBadHandle() {
            if (_handle.IsClosed || _handle.IsInvalid) {
                throw new InvalidOperationException("Invalid Handle");
            }
        }
        private void _throwIfError() {
            if (ErrorState != PageBufferErrorStates.None) {
                throw new InvalidOperationException(String.Format("Page Buffer Errored: {0}", ErrorState));
            }
        }
        private void _checkConstructor(SafeFileHandle handle, int alignment, int blockSize, int blocksPerPage) {
            if (handle.IsClosed || handle.IsInvalid) {
                throw new ArgumentException("Bad Handle");
            }
            if (handle == null) {
                throw new ArgumentNullException("handle");
            }
            if (alignment < 1) {
                throw new ArgumentException("Invalid alignment");
            }
            if (blockSize < 1) {
                throw new ArgumentException("Invalid blockSize");
            }
            if (blocksPerPage < 1) {
                throw new ArgumentException("Invalid blocksPerPage");

            }
            if (blockSize * blocksPerPage > 1000000000) {
                throw new ArgumentException("Cannot create buffer");
            }
        }
        public UnmanagedMemoryAccessor AccessBlock(UInt64 blockIdx, FileAccess method = FileAccess.Read) {
            _throwIfBadHandle();
            _throwIfError();
            if (!_isBlockInPage(blockIdx)) {
                if (_needsFlush) {
                    if (!_writeTo(_pageIdx)) {
                        throw new InvalidOperationException("Flush Failed");
                    }
                }
                if (!_readFrom(_indexOfPageContaining(blockIdx))) {
                    throw new InvalidOperationException("Read Failed");
                }
            }
            if (method.HasFlag(FileAccess.Write)) {
                _needsFlush = true;
            }
            return new UnmanagedMemoryAccessor(_page, _firstByteInBlock(blockIdx), _blockSize, method);
        }
        public UnmanagedMemoryStream StreamBlock(UInt64 block, FileAccess method = FileAccess.Read) {
            _throwIfBadHandle();
            _throwIfError();
            if (!_isBlockInPage(block)) {
                if (_needsFlush) {
                    if (!_writeTo(_pageIdx)) {
                        throw new InvalidOperationException("Flush Failed");
                    }
                }
                if (!_readFrom(_indexOfPageContaining(block))) {
                    throw new InvalidOperationException("Read Failed");
                }
            }
            if (method.HasFlag(FileAccess.Write)) {
                _needsFlush = true;
            }
            return new UnmanagedMemoryStream(_page, _firstByteInBlock(block), _blockSize, method);
        }
        public PageCache(SafeFileHandle handle, Int32 alignment, Int32 blockSize, Int32 blocksPerPage) {
            _checkConstructor(handle, alignment, blockSize, blocksPerPage);
            _handle = handle;
            _pageSize = (uint)(blockSize * blocksPerPage);
            _page = new AlignedNativeBuffer(blockSize * blocksPerPage, alignment);
            _pageIdx = 0;
            _needsFlush = false;
            _blockSize = (uint)blockSize;
            _readFrom(0);
        }
    }
}
