using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Menus
{
    public class HashMenu<T> : HashSet<IItem<T>>
    {
        private static readonly IEqualityComparer<IItem<T>> _itemEquater = new DelegateEqualityComparer<IItem<T>>((x, y) =>
          {
              if (x == null && y == null) return true;
              else if (x == null) return false;
              else if (y == null) return false;
              else return x.Key == y.Key;
          }, (obj) =>
          {
              if (obj == null) return 0;
              return obj.Key.GetHashCode();
          });
        public HashMenu() : base(_itemEquater) { }
    }
}
