using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Menus
{
    public interface IItem<TValue>
    {
        string Key { get; }
        string Text { get; }
        int Ordinal { get; }
        TValue Value { get; }
    }
}
