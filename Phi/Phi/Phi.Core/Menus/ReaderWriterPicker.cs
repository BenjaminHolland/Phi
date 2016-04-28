using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Phi.Menus
{
    public class ReaderWriterPicker : IPicker
    {
        private readonly TextReader _reader;
        private readonly TextWriter _writer;
        private readonly bool _ownsReader;
        private readonly bool _ownsWriter;
        private readonly string _header;
        private readonly string _prompt;
        public ReaderWriterPicker(TextReader reader,TextWriter writer,bool ownsReader,bool ownsWriter,string header,string prompt)
        {
            _reader = reader;
            _writer = writer;
            _ownsReader = ownsReader;
            _ownsWriter = ownsWriter;
            _header = header;
            _prompt = prompt;
        }
        public IEnumerable<TValue> Pick<TValue>(ISet<IItem<TValue>> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (items.Count == 0) throw new ArgumentEmptyException(nameof(items));
            if (_writer != null)
            {
                _writer.WriteLine(_header);
                foreach(var item in items.OrderBy(i=>i.Ordinal))
                {
                    _writer.WriteLine($"{item.Key}. {item.Text}");
                }
                _writer.Write(_prompt);
            }
            string key = _reader.ReadLine();
            var result = items.Where(i => i.Key == key);
            return result.Select(i=>i.Value);
        }
    }
}
