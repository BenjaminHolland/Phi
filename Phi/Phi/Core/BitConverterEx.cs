using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core {
    public enum ByteOrder {
        LittleEndian,
        BigEndian
    }
    public static class BitConverterEx {
        public static readonly ByteOrder SystemByteOrder = _getSystemByteOrder();
        private static ByteOrder _getSystemByteOrder() {
            return BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
        }
        private static void _checkConversion(byte[] bytes, int startIdx, int requiredBytes) {
            if (bytes == null) {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length <= startIdx) {
                throw new ArgumentNullException("startIdx");
            }
            if (bytes.Length - startIdx < requiredBytes) {
                throw new ArgumentException("bytes: Insufficient Length");
            }

        }
        private static bool _needsReverse(ByteOrder order) {
            return order != SystemByteOrder;
        }
        
        public static UInt32 ToUInt32(byte[] bytes, int startIdx, ByteOrder order) {
            _checkConversion(bytes, startIdx, 4);
            UInt32 ret;
            if (!_needsReverse(order)) {
                ret = (UInt32)(bytes[startIdx] << 24 |
                             bytes[startIdx + 1] << 16 |
                             bytes[startIdx + 2] << 8 |
                             bytes[startIdx + 3]);
            }
            else {
                ret = (UInt32)(bytes[startIdx] |
                               bytes[startIdx + 1] << 8 |
                               bytes[startIdx + 2] << 16 |
                               bytes[startIdx + 3] << 24);
            }
            return ret;
        }
        public static UInt32 ToUInt24(byte[] bytes, int startIdx, ByteOrder order) {
            _checkConversion(bytes, startIdx, 3);
            UInt32 ret = 0;
            if (!_needsReverse(order)) {
                ret = (UInt32)(bytes[startIdx] << 16 |
                               bytes[startIdx + 1] << 8 |
                               bytes[startIdx + 2]);
            }
            else {
                ret = (UInt32)(bytes[startIdx] |
                               bytes[startIdx + 1] << 8 |
                               bytes[startIdx + 2] << 16);
            }
            return ret;
        }
        public static UInt16 ToUInt16(byte[] bytes, int startIdx, ByteOrder order) {
            _checkConversion(bytes, startIdx, 2);
            UInt16 ret = 0;
            if (!_needsReverse(order)) {
                ret = (UInt16)(bytes[startIdx] << 8 |
                               bytes[startIdx + 1]);
            }
            else {
                ret = (UInt16)(bytes[startIdx] |
                               bytes[startIdx + 1] << 8);
            }
            return ret;
        }
        public static Int32 ToInt32(byte[] bytes, int startIdx, ByteOrder order) {
            _checkConversion(bytes, startIdx, 4);
            Int32 ret;
            if (!_needsReverse(order)) {
                ret = (Int32)(bytes[startIdx] << 24 |
                             bytes[startIdx + 1] << 16 |
                             bytes[startIdx + 2] << 8 |
                             bytes[startIdx + 3]);
            }
            else {
                ret = (Int32)(bytes[startIdx] |
                               bytes[startIdx + 1] << 8 |
                               bytes[startIdx + 2] << 16 |
                               bytes[startIdx + 3] << 24);
            }
            return ret;
        }
        public static Int32 ToInt24(byte[] bytes, int startIdx, ByteOrder order) {
            _checkConversion(bytes, startIdx, 3);
            Int32 ret = 0;
            if (!_needsReverse(order)) {
                ret = (Int32)(bytes[startIdx] << 16 |
                               bytes[startIdx + 1] << 8 |
                               bytes[startIdx + 2]);
                if ((bytes[startIdx + 2] & 0x80)!=0) {
                    ret |= 0xff << 24;
                }
            }
            else {
                ret = (Int32)(bytes[startIdx] |
                               bytes[startIdx + 1] << 8 |
                               bytes[startIdx + 2] << 16);
                if ((bytes[startIdx] & 0x80) !=0) {
                    ret |= 0xff << 24;
                }
            }
            return ret;
        }
        public static Int16 ToInt16(byte[] bytes, int startIdx, ByteOrder order) {
            _checkConversion(bytes, startIdx, 2);
            Int16 ret = 0;
            if (!_needsReverse(order)) {
                ret = (Int16)(bytes[startIdx] << 8 |
                               bytes[startIdx + 1]);
            }
            else {
                ret = (Int16)(bytes[startIdx] |
                              bytes[startIdx + 1] << 8);
            }
            return ret;
        }


      

    }
}
