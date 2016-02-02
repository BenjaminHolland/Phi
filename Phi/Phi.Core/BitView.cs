using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core {
    public sealed class BitView {
        private byte[] _bytes;
        private int _bitOffset;
        private int _bitLength;
        public int BitOffset {
            get {
                return _bitOffset;
            }
        }
        public int BitLength {
            get {
                return _bitLength;
            }
        }
        public BitView(byte[] bytes, int bitOffset, int bitLength) {
            _bytes = bytes;
            _bitLength = bitLength;
            _bitOffset = bitOffset;
        }
        public BitView(int size, bool fill) {
            _bitOffset = 0;
            _bitLength = size * 8;
            _bytes = new byte[size];
            if (!fill) return;
            for (int i = 0; i < size; i++) {
                _bytes[i] = 0xff;
            }
        }
        public bool GetBit(int bit) {
            bit = _bitOffset + bit;
            return (_bytes[(bit / 8)] & (1 << (7 - (bit % 8)))) != 0;
        }

        public void SetBit(int bit) {
            bit = _bitOffset + bit;
            _bytes[(bit / 8)] |= unchecked((byte)(1 << (7 - (bit % 8))));
        }

        public void ClearBit(int bit) {
            bit = _bitOffset + bit;
            _bytes[(bit / 8)] &= unchecked((byte)~(1 << (7 - (bit % 8))));
        }
        public bool this[int bit] {
            get { return GetBit(bit); }
            set { if (value) SetBit(bit); else ClearBit(bit); }
        }
        public byte[] GetBytes(int size, bool fill) {
            if (size == 0) size = (_bitLength + 7) / 8;
            BitView bitField = new BitView(size, fill);
            for (int src = 0, dst = (size * 8) - _bitLength; src < _bitLength && dst < (size * 8); src++, dst++) {
                bitField[dst] = GetBit(src);
            }
            return bitField._bytes;
        }
    }

}
