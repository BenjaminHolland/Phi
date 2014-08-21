using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core.Development {
    [AttributeUsage(AttributeTargets.Method|
                    AttributeTargets.Interface|
                    AttributeTargets.Class|
                    AttributeTargets.Struct,AllowMultiple=true,Inherited=false)]
    public class TODOAttribute:Attribute {
        public string TaskMessage {
            get;
            private set;
        }
        public TODOAttribute(string task)
            : base() {
                TaskMessage = task;
        }
    }
}
