using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi
{
    public static class IEnumerableTExt
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> self,int chunkSize)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            for(int i = 0; i < self.Count(); i+=chunkSize)
            {
                yield return self.Skip(i).Take(Math.Min(chunkSize, self.Count() - i));
            }
        }
    }
}
