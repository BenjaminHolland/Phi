using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core
{
    public static class StringExt
    {
        private static readonly Dictionary<char, string> _escapeLookup = new Dictionary<char, string>()
        {
            { '\n',"\\n"},
            { '\r',"\\r"}
        };
        public static string AsString(this IEnumerable<char> self)
        {
            return new string(self.ToArray());
        }
        public static string Unescape(this string self)
        {
            string temp = self;
            foreach(var escape in _escapeLookup)
            {
                temp = temp.Replace(escape.Key.ToString(), escape.Value);
            }
            return temp;
        }
        public static SecureString ToSecure(this string self)
        {
            SecureString ret = new SecureString();
            foreach(var c in self)
            {
                ret.AppendChar(c);
            }
            return ret;
        }

        [CodeSource(source:"Fabio Pintos", site:@"https://blogs.msdn.microsoft.com/fpintos/2009/06/12/how-to-properly-convert-securestring-to-string/")]
        public static string ToUnsecure(this SecureString self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(self);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }
    }
}
