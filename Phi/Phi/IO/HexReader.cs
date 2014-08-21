using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Phi.Core;
using Phi.IO.Parsing;
namespace Phi.IO {
    public class HexReader : StreamReader {
        private byte[] _buffer;
        private void buffer(int requiredBytes) {
            if (BaseStream.Read(_buffer, 0, requiredBytes)!=requiredBytes) {
                throw new System.IO.EndOfStreamException();
            }
        }
        public HexReader(string hexString)
            : base(new MemoryStream(HexParser.ToByteArray(hexString), false)) {
            _buffer = new byte[4];
        }
        
        public UInt32 ReadUInt32() {
            buffer(4);
            return BitConverterEx.ToUInt32(_buffer, 0, BitConverterEx.SystemByteOrder);
        }
        public UInt32 ReadUInt24() {
            buffer(3);
            return BitConverterEx.ToUInt24(_buffer, 0, BitConverterEx.SystemByteOrder);
        }
        public UInt16 ReadUInt16() {
            buffer(2);
            return BitConverterEx.ToUInt16(_buffer, 0, BitConverterEx.SystemByteOrder);
        }

        public UInt32 ReadUInt32(ByteOrder nextBytesAre) {
            buffer(4);
            return BitConverterEx.ToUInt32(_buffer, 0, nextBytesAre);
        }
        public UInt32 ReadUInt24(ByteOrder nextBytesAre) {
            buffer(3);
            return BitConverterEx.ToUInt24(_buffer, 0, nextBytesAre);
        }
        public UInt16 ReadUInt16(ByteOrder nextBytesAre) {
            buffer(2);
            return BitConverterEx.ToUInt16(_buffer, 0, nextBytesAre);
        }
        
        public Int32 ReadInt32() {
            buffer(4);
            return BitConverterEx.ToInt32(_buffer, 0, BitConverterEx.SystemByteOrder);
        }
        public Int32 ReadInt24() {
            buffer(3);
            return BitConverterEx.ToInt24(_buffer, 0, BitConverterEx.SystemByteOrder);
        }
        public Int16 ReadInt16() {
            buffer(2);
            return BitConverterEx.ToInt16(_buffer, 0, BitConverterEx.SystemByteOrder);
        }

        public Int32 ReadInt32(ByteOrder nextBytesAre) {
            buffer(4);
            return BitConverterEx.ToInt32(_buffer, 0, nextBytesAre);
        }
        public Int32 ReadInt24(ByteOrder nextBytesAre) {
            buffer(3);
            return BitConverterEx.ToInt24(_buffer, 0, nextBytesAre);
        }
        public Int16 ReadInt16(ByteOrder nextBytesAre) {
            buffer(2);
            return BitConverterEx.ToInt16(_buffer, 0, nextBytesAre);
        }
      
        public Byte ReadByte() {
            buffer(1);
            return _buffer[0];
        }      
        public SByte ReadSByte() {
            buffer(1);
            return (SByte)_buffer[0];
        }

        
    }
}
