using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Files.RIFF {
    public enum RIFFState {
        ReadyForData,
        CollectingData,
        ReadyForSerialization
    };

    
}
