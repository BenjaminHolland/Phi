using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Devices.ReactivePort {
    public sealed class ReactivePortPacket {
        public readonly byte[] Data;
        public ReactivePortPacket(byte[] data) {
            Data = data;
        }

    }
}
