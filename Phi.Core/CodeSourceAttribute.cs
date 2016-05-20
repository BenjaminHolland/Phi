using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class CodeSourceAttribute : Attribute
    {
        private readonly string _site;
        private readonly string _source;
        private readonly string _licence;
        public Uri Site
        {
            get
            {
                return new Uri(_site);
            }
        }
        public  string Source
        {
            get
            {
                return _source;
            }
        }
        public string Licence
        {
            get
            {
                return _licence;
            }
        }
        public CodeSourceAttribute(string source = "Unknown", string licence = "Unknown", string site = null)
        {
            if (licence == null) licence = "Unknown";
            if (source == null) source = "Unknown";
            _site = site;
            _licence = licence;
            _source = source;
        }
    }
}
