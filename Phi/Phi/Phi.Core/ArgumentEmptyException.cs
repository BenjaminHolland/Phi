using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi
{
    public class ArgumentEmptyException:ArgumentException
    {
        public ArgumentEmptyException() : base("Argument is empty.") { }
        public ArgumentEmptyException(string paramName):base("Argument is empty.",paramName){ }
        public ArgumentEmptyException(string paramName, Exception innerException) : base("Argument is empty.", paramName, innerException) { }
    }
}
