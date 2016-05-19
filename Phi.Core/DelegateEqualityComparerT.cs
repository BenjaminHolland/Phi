using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core
{
    public class DelegateEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equate;
        private readonly Func<T, int> _hash;
        public DelegateEqualityComparer(Func<T,T,bool> equate,Func<T,int> hash)
        {
            if (equate == null) throw new ArgumentNullException(nameof(equate));
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            _equate = equate;
            _hash = hash;
        }
        public bool Equals(T x, T y)
        {
            return _equate(x, y);
        }
        public int GetHashCode(T obj)
        {
            return _hash(obj);
        }
    }
}
