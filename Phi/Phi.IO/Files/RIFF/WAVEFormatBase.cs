using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF {
    public abstract class WAVEFormatBase : RIFFBase {
        private ushort _formatTag;
        private ushort _channels;
        private uint _sampleRate;
        public abstract ushort BlockAlign {
            get;
        }
        public abstract uint ByteRate {
            get;
        }
        public uint SampleRate {
            get {
                return _sampleRate;
            }
            private set {
                _sampleRate = value;
            }
        }
        public RIFFWaveType Format {
            get {
                return (RIFFWaveType)_formatTag;
            }
            private set {
                _formatTag = (ushort)value;
            }
        }
        public ushort Channels {
            get {
                return _channels;
            }
            private set {
                _channels = value;
            }
        }
        protected override Stream CreateBuffer() {
            return new MemoryStream();
        }
        protected override void DestroyBuffer() {
            Buffer.Dispose();
        }
        protected void SerializeCommonFormatData(BinaryWriter bw) {
            bw.Write((ushort)Format);
            bw.Write(Channels);
            bw.Write(SampleRate);
            bw.Write(ByteRate);
            bw.Write(BlockAlign);
        }
        public virtual void Commit() {
            using (BeginData()) {
                using (var bw = new BinaryWriter(Buffer, Encoding.ASCII, true)) {
                    SerializeCommonFormatData(bw);
                }
            }
        }
        public override uint Size {
            get {
                RequireState(RIFFState.ReadyForSerialization);
                return (uint)Buffer.Length;
            }
        }
        protected override void OnSerialize(BinaryWriter writer) {
            //SerializeHeader(writer);
            Buffer.CopyTo(writer.BaseStream);
        }
        public WAVEFormatBase(RIFFWaveType waveType, ushort channels, uint sampleRate) {
            ChunkId = "fmt";
            Format = waveType;
            Channels = channels;
            SampleRate = sampleRate;
        }
    }
}
