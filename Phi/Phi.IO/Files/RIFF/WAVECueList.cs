using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF.WAVE {
    public class WAVECueList : RIFFBase {
        public WAVECueList() : base() {
            ChunkId = "cue";
        }
        public override uint Size {
            get {
                return (uint)Buffer.Length;
            }
        }
        private uint _cueCount = 0;
        public void WriteCuePoint(uint position,string chunkName,uint chunkStart,uint blockStart,uint sampleOffset) {
            RequireState(RIFFState.CollectingData);
            uint chunkId = RIFFBase.ToFourCC(chunkName);
            using (var bw = new BinaryWriter(Buffer, Encoding.ASCII, true)) {
                bw.Write(_cueCount);
                bw.Write(position);
                bw.Write(chunkId);
                bw.Write(chunkStart);
                bw.Write(blockStart);
                bw.Write(sampleOffset);
                _cueCount++;
                Buffer.Seek(0, SeekOrigin.Begin);
                bw.Write(_cueCount);
                Buffer.Seek(0, SeekOrigin.End);
            }

        }
        protected override Stream CreateBuffer() {
            var ms = new MemoryStream();
            ms.Write(new byte[] { 0, 0, 0, 0 }, 0, 4);
            return ms;
        }

        protected override void DestroyBuffer() {
            Buffer.Dispose();
        }

        protected override void OnBeginData() {
            State = RIFFState.CollectingData;
        }

        protected override void OnDispose() {
            return;
        }

        protected override void OnEndData() {
            State = RIFFState.ReadyForSerialization;
        }

        protected override void OnSerialize(BinaryWriter writer) {
            Buffer.CopyTo(writer.BaseStream);
        }
    }
}
