using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF.WAVE {
    public class WAVEPcmFormat : WAVEFormatBase {
        public WAVEPcmFormat(ushort channels, uint sampleRate, ushort bitDepth) :
            base(RIFFWaveType.PCM, channels, sampleRate) {
            BitDepth = bitDepth;
        }
        public ushort BitDepth {
            get;
            private set;
        }
        public override ushort BlockAlign {
            get {
                return (ushort)(Channels * (BitDepth / 8));
            }
        }
        public override uint ByteRate {
            get {

                return (uint)(SampleRate * Channels * (BitDepth / 8));
            }
        }
        public override void Commit() {
            using (BeginData()) {
                using (var bw = new BinaryWriter(Buffer, Encoding.ASCII, true)) {
                    SerializeCommonFormatData(bw);
                    bw.Write(BitDepth);
                }
            }
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
    }
}
