using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF {
    public class RIFFList : RIFFContainer {
        public RIFFList() : base() {
            ChunkId = "LIST";
        }
    }
}
