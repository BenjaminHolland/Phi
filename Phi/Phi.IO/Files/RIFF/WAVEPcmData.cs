using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF {
    public class WAVEPcmData : WAVEDataBase {
        private ushort _sampleSize;
        protected void RequireSampleSize(ushort requiredSampleSize) {
            if (_sampleSize != requiredSampleSize) throw new InvalidOperationException();
        }
        public WAVEPcmData(WAVEPcmFormat format) : base(format) {
            _sampleSize = (ushort)(format.BitDepth / 8);
        }
        private BinaryWriter _sampleWriter;
        protected override Stream CreateBuffer() {
            _sampleWriter = new BinaryWriter(base.CreateBuffer(), Encoding.ASCII, true);
            return _sampleWriter.BaseStream;
        }
        protected override void DestroyBuffer() {
            _sampleWriter.Dispose();
            base.DestroyBuffer();
        }
        public void WriteSample(byte sample) {
            RequireState(RIFFState.CollectingData);
            RequireSampleSize(1);
            _sampleWriter.Write(sample);
        }
        public void WriteSamples(byte[] samples, int offset, int count) {
            RequireState(RIFFState.CollectingData);
            RequireSampleSize(1);
            _sampleWriter.Write(samples, offset, count);
        }
        public void WriteSamples(byte[] samples) {
            _sampleWriter.Write(samples, 0, samples.Length);
        }
        public void WriteSample(short sample) {
            RequireState(RIFFState.CollectingData);
            RequireSampleSize(2);
            _sampleWriter.Write(sample);
        }
        public void WriteSamples(short[] samples, int offset, int count) {
            RequireState(RIFFState.CollectingData);
            RequireSampleSize(2);
            foreach (var sample in samples) {
                _sampleWriter.Write(sample);
            }
        }
        public void WriteSamples(short[] samples) {
            WriteSamples(samples, 0, samples.Length);
        } 
    }
}
