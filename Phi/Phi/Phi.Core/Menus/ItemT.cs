using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Menus
{
    public class Item<T> : IItem<T>
    {
        public string Key{get;private set;}
        public int Ordinal { get; private set; }
        public string Text { get; private set; }
        public T Value { get; private set; }
        public Item(string key,string text,int ordinal,T value)
        {
            if (key == null) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentEmptyException(nameof(text));
            if (text == null) throw new ArgumentNullException(nameof(text));
            Key = key;
            Text = text;
            Ordinal = ordinal;
            Value = value;
        }
    }
}
