using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi
{
    public static class BaseConverter
    {
        private static Dictionary<string,byte> GenerateBase16EncoderLookup()
        {
            Dictionary<string, byte> lookup = new Dictionary<string, byte>();
            for(int i = 0; i <= 255; i++)
            {
                lookup.Add(i.ToString("X2"), unchecked((byte)i));
            }
            return lookup;
        }
        private static readonly Lazy<Dictionary<string, byte>> _base16EncoderLookup = new Lazy<Dictionary<string, byte>>(GenerateBase16EncoderLookup); 
        public static string DecodeBase64String(byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }
        public static byte[] EncodeBase64String(string data)
        {
            return Convert.FromBase64String(data);
        }
        public static string DecodeBase16String(byte[] buffer)
        {
            return buffer.Aggregate(new StringBuilder(), (a, v) => a.Append(v.ToString("X2")), (a) => a.ToString());
        }
        public static byte[] EncodeBase16String(string buffer)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (buffer.Length == 0) throw new ArgumentEmptyException();
            if (buffer.Length % 2 != 0) throw new ArgumentException("Invalid base 16 string.");
            return buffer.Chunk(2)
                .Select(chs => new string(chs.ToArray()).ToUpper())
                .Select(chunk=>_base16EncoderLookup.Value[chunk]).ToArray();
            
        }
    }
}
