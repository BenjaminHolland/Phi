using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Mathmatics {
    public static class MathHelper {
        public static double GetPercent(int partial, int total) {
            return (partial / (double)total) * 100.0;
        }
        
    }
}
