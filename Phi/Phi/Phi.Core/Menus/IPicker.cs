using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Menus
{
    public interface IPicker
    {
        IEnumerable<TValue> Pick<TValue>(ISet<IItem<TValue>> items);
    }
}
