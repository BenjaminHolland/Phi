using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF.WAVE {
    public class WAVEDataBase : RIFFBase {
        public WAVEDataBase(WAVEFormatBase format) {
            ChunkId = "data";
            BlockSize = format.BlockAlign;
        }
        public uint BlockSize {
            get;
            private set;
        }
        protected void EnsureBlockSize(uint requiredBlockSize,uint actualBlockSize) {
            if (actualBlockSize != requiredBlockSize) throw new InvalidOperationException();
        }
        protected void RequireBlockSize(uint requiredBlockSize) {
            if (BlockSize != requiredBlockSize) throw new InvalidOperationException();
        }
        public void WriteBlock(byte[] block) {
            RequireState(RIFFState.CollectingData);
            EnsureBlockSize(BlockSize, (uint)block.Length);
            Buffer.Write(block, 0, block.Length);
        }
        public override uint Size {
            get {
                RequireState(RIFFState.ReadyForSerialization);
                return (uint)Buffer.Length;
            }
        }

        protected override Stream CreateBuffer() {
            return File.Create(Path.GetTempFileName());
        }

        protected override void DestroyBuffer() {
            FileStream stream = Buffer as FileStream;
            string name = stream.Name;
            stream.Dispose();
            File.Delete(name);
        }

        protected override void OnBeginData() {
            State = RIFFState.CollectingData;
        }

        protected override void OnDispose() {
            return;
        }

        protected override void OnEndData() {
            if (Buffer.Length % 2 != 0) {
                Buffer.Seek(0, SeekOrigin.End);
                Buffer.Write(new byte[] { 0 }, 0, 1);
                Buffer.Seek(0, SeekOrigin.Begin);
            }

            State = RIFFState.ReadyForSerialization;
        }

        protected override void OnSerialize(BinaryWriter writer) {
            Buffer.CopyTo(writer.BaseStream);
        }
    }

}
