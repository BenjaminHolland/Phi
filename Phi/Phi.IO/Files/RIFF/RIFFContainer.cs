using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF {
    public class RIFFContainer : RIFFBase {
        private List<RIFFBase> _children = new List<RIFFBase>();
        public virtual void AddChild(RIFFBase child) {
            RequireState(RIFFState.CollectingData);
            _children.Add(child);
        }
        public T FindChild<T>(string chunkId) where T : RIFFBase {
            RequireState(RIFFState.ReadyForSerialization);
            return _children.FirstOrDefault(child => child.ChunkId == chunkId) as T;
        }
        public override uint Size {
            get {
                RequireState(RIFFState.ReadyForSerialization);
                uint size = (uint)Buffer.Length;
                if (_children.Count != 0) {
                    size += (uint)_children.Sum(child => child.Size + 8);
                }
                if (_children.Count == 0) {
                    if (size % 2 != 0) {
                        size++;
                    }
                }
                return size;
            }
        }
        protected override void OnSerialize(BinaryWriter writer) {
            Buffer.CopyTo(writer.BaseStream);
            foreach (var child in _children) {
                child.Serialize(writer);
            }
        }

        protected override Stream CreateBuffer() {
            return new MemoryStream();
        }

        protected override void DestroyBuffer() {
            Buffer.Dispose();
        }

        protected override void OnBeginData() {
            State = RIFFState.CollectingData;
            return;
        }

        protected override void OnDispose() {
            return;
        }

        protected override void OnEndData() {
            State = RIFFState.ReadyForSerialization;
        }
    }
}
