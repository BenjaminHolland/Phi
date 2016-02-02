using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF {
    /// <summary>
    /// Contains low level infrastructure for building RIFF files.
    /// </summary>
    public abstract class RIFFBase : IDisposable {
        /// <summary>
        /// Convert a RIFF FourCC into a string.
        /// </summary>
        /// <param name="fourCC"></param>
        /// <returns></returns>
        public static string FromFourCC(uint fourCC) {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(fourCC));
        }
        /// <summary>
        /// Converts a string into a RIFF FourCC
        /// </summary>
        /// <param name="fourCC"></param>
        /// <returns></returns>
        public static uint ToFourCC(string fourCC) {
            if (string.IsNullOrWhiteSpace(fourCC)) throw new ArgumentException();
            if (fourCC.Length > 4) throw new ArgumentException();
            if (fourCC.Length < 4) fourCC=fourCC.PadRight(4, ' ');
            return BitConverter.ToUInt32(Encoding.ASCII.GetBytes(fourCC),0);
        }
        /// <summary>
        /// Class that acts as a token for this chunkgs data aquisition phase.
        /// </summary>
        private class CallbackDisposable : IDisposable {
            private Action _onDisposed;
            public CallbackDisposable(Action onDisposed) {
                _onDisposed = onDisposed;
            }
            private bool _isDisposed;
            public void Dispose() {
                if (_isDisposed) return;
                _onDisposed();
                _isDisposed = true;
            }
        }
        public bool IsDisposed {
            get;
            private set;
        }
        /// <summary>
        /// Calling classes should override this to do specialized cleanup.
        /// Preconditions: Buffer is still accessable and contains the latest data. Chunk is in ReadyForSerialization State.
        /// Postconditions: Buffer is ready to be destroyed, specialized cleanup is complete.
        /// </summary>
        protected abstract void OnDispose();
        /// <summary>
        /// Disposes this chunk.
        /// </summary>
        public void Dispose() {
            if (IsDisposed) return;
            _dataToken.Dispose();
            OnDispose();
            DestroyBuffer();
            IsDisposed = true;
        }


        private IDisposable _dataToken;
        /// <summary>
        /// Starts data aquisition for this chunk. data aquisition ends when the returned disposable object is disposed.
        /// </summary>
        /// <returns></returns>
        public IDisposable BeginData() {
            RequireState(RIFFState.ReadyForData);
            if (Buffer != null) {
                DestroyBuffer();
            }
            Buffer = CreateBuffer();
            OnBeginData();
            _dataToken = new CallbackDisposable(EndData);
            return _dataToken;
        }
        private void EndData() {
            Buffer.Seek(0, SeekOrigin.Begin);
            OnEndData();

            _dataToken = null;
        }
        /// <summary>
        /// Inheriting classes should override this to provide preperation for the data aquisition phase.
        /// State Before Called: Buffer has been created.
        /// State Requirements After Call: State has been set.
        /// </summary>
        protected abstract void OnBeginData();
        protected abstract void OnEndData();

        private RIFFState _state;
        public RIFFState State {
            get {
                return _state;
            }
            protected set {
                _state = value;
            }
        }

        protected void RequireState(RIFFState state) {
            if (State != state) throw new InvalidOperationException();
        }

        public void Serialize(BinaryWriter writer) {
            RequireState(RIFFState.ReadyForSerialization);
            SerializeHeader(writer);
            bool needsPad = Size % 2 != 0;
            OnSerialize(writer);
            if (needsPad) {
                writer.Write((byte)0);
            }
            State = RIFFState.ReadyForData;
        }
        protected abstract void OnSerialize(BinaryWriter writer);

        protected abstract Stream CreateBuffer();
        protected abstract void DestroyBuffer();
        protected Stream Buffer {
            get;
            private set;
        }
        public void Write(string buffer) {
            byte[] b = Encoding.ASCII.GetBytes(buffer);
            Write(b, 0, b.Length);
        }
        public void Write(byte[] buffer, int offset, int count) {
            RequireState(RIFFState.CollectingData);
            Buffer.Write(buffer, offset, count);
        }
        public abstract uint Size {
            get;
        }
        private uint _chunkId;
        public string ChunkId {
            get {
                return FromFourCC(_chunkId);
            }
            set {
                _chunkId = ToFourCC(value);
            }
        }

        public void SerializeHeader(BinaryWriter writer) {
            writer.Write(_chunkId);
            writer.Write(Size);
        }
    }
}
