using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Win32
{
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
}
