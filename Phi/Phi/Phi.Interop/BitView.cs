using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Phi;
namespace Phi.Interop {



    public class BitView : IBitView {
        protected byte[] Source {
            get;
            set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool GetBit(int bit) {
            bit += Offset;
            return (Source[bit / 8] & unchecked((byte)(1 << (7 - (bit % 8))))) != 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetBit(int bit) {
            bit += Offset;
            Source[bit / 8] |= unchecked((byte)(1 << (7 - (bit % 8))));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void ClearBit(int bit) {
            bit += Offset;
            Source[bit / 8] &= unchecked((byte)~(1 << (7 - (bit % 8))));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AssignBit(int bit, bool value) {
            if (value) SetBit(bit); else ClearBit(bit);
        }
        [System.Runtime.CompilerServices.IndexerName("BitIndexer")]
        public bool this[int bit] {
            get {
                return GetBit(bit);
            }

            set {
                AssignBit(bit, value);
            }
        }
        public IEnumerable<bool> Bits {
            get {
                return Enumerable.Range(0, Length).Select(i => this[i]);
            }
        }
        public int Length {
            get;
            protected set;
        }
        public int Offset {
            get;
            protected set;
        }
        public BitView(byte[] source, int offset, int length) {
            if (source == null) throw new ArgumentNullException("source");
            int requiredBits = offset + length;
            if (source.Length*8 < requiredBits) throw new ArgumentEmptyException("source");
            if (length <= 0 ) throw new ArgumentOutOfRangeException("length"); 
            if (offset < 0 ) throw new ArgumentOutOfRangeException("offset");
            Offset = offset;
            Length = length;
            Source = source;
        }
        public BitView(byte[] source) : this(source, 0, source.Length * 8) { }
        public BitView(int length):this(new byte[(length+7)/8]) { }
        public BitView(IEnumerable<bool> bits) {
            if (bits == null) throw new ArgumentNullException("bits");
            if (!bits.Any()) throw new ArgumentEmptyException("bits");
            var bitArray = bits.ToArray();
            var tmp=new BitView(bitArray.Length);
            Offset = 0;
            Length = bitArray.Length;
            for(int i = 0; i < bitArray.Length; i++) {
                tmp[i] = bitArray[i];
            }
            Source = tmp.Source;
        } 
        public int Read(IBitView src, int pRead, int pWrite, int length) {
            int requiredSrcBits = pRead + length;
            int requiredDstBits = pWrite + length;
            if (requiredDstBits > Length) throw new InvalidOperationException("Insufficient Data");
            if (requiredSrcBits > src.Length) throw new InvalidOperationException("Insufficient Data");
            for (int i = 0; i < length; i++) {
                this[i + pWrite] = src[i + pRead];
            }
            return length;
        }
        public int Write(IBitView dst, int pRead, int pWrite, int length) {
            int requiredSrcBits = pRead + length;
            int requiredDstBits = pWrite + length;
            if (requiredDstBits > dst.Length) throw new InvalidOperationException();
            if (requiredSrcBits > Length) throw new InvalidOperationException();
            for (int i = 0; i < length; i++) {
                dst[i + pWrite] = this[i + pRead];
            }
            return length;
        }        
        public IBitView CreateSubview(int offset,int length) {
            return new BitView(Source, Offset+offset, length);
        }
        public byte[] GetAlignedBytes(int align, bool fill) {
            if (align < 0) throw new ArgumentOutOfRangeException("align");
            if (align == 0) align = (Length + 7) / 8;
            BitView bytes = new BitView(Enumerable.Range(0, align).Select(i => fill ? (byte)0xff : (byte)0).ToArray());
            int dstLength = align * 8;
            for (int src = 0, dst = dstLength - Length; src < Length && dst < dstLength; src++, dst++) {
                bytes[dst] = this[src];
            }
            return bytes.Source;
        }
    }
}