using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Interop
{
    public static class ByteArrayExt
    {
        public static IBitView MakeView(this byte[] self,int offset,int length)
        {
            return new BitView(self, offset, length);
        }
        public static IBitView MakeView(this byte[] self)
        {
            return new BitView(self);
        }
        
    }
}
