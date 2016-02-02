using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core.Development{
    [AttributeUsage(AttributeTargets.All,AllowMultiple =true)]
    public sealed class CodeSourceAttribute:Attribute {
        public readonly string Url;
        public readonly string Source;
        public readonly string Licence;
        public CodeSourceAttribute(string source,string licence,string url=null) {
            Url = url;
            Source = source;
            Licence = licence;
        }
    }
}
