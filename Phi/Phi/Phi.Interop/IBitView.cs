using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Interop {
    public interface IBitView {
        /// <summary>
        /// The length of the blob in bits.
        /// </summary>
        int Length { get; }
        /// <summary>
        /// The offset in bits from the beginning of the source.
        /// </summary>
        int Offset { get; }
        [System.Runtime.CompilerServices.IndexerName("BitIndexer")]
        bool this[int bit] { get; set; }
        IEnumerable<bool> Bits {
            get;
        }
        /// <summary>
        /// Write data to another blob.
        /// </summary>
        /// <param name="dst">The destination for the data.</param>
        /// <param name="dstIdx">The bit offset in the destination blob to start writing to.</param>
        /// <param name="thisIdx">The bit offset in this blob to start reading from.</param>
        /// <param name="length">The number of bits to transfer.</param>
        /// <returns>The number of bits transferred</returns>
        int Write(IBitView dst, int dstIdx, int thisIdx, int length);
        IBitView CreateSubview(int offset, int length);
        /// <summary>
        /// Reads data from another blob.
        /// </summary>
        /// <param name="src">The source of the data.</param>
        /// <param name="srcIdx">The bit offset in the source blob to start reading from.</param>
        /// <param name="thisIdx">The bit offset in this blob to start writing to.</param>
        /// <param name="length">The number of bits to transfer</param>
        /// <returns>The number of bits transferred</returns>
        int Read(IBitView src, int srcIdx, int thisIdx, int length);
        /// <summary>
        /// Gets an array of bytes that contains the data for this section.
        /// </summary>
        /// <param name="align">The number of bytes to return. Passing 0 will return an array with the least number of bytes possible used to represent the value.</param>
        /// <param name="fill">If set, the returned bytes will be 1-filled before copying in the data.</param>
        byte[] GetAlignedBytes(int align, bool fill);
    }
}
