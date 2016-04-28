using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Win32
{
    [Flags]
    public enum NativeShareMode : uint
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4,
    }
}
