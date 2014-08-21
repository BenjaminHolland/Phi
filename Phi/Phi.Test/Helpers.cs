using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Test {
    public static class Helpers {
        static Helpers() {
            UpperLetters = new HashSet<char>();
            LowerLetters = new HashSet<char>();
            Numbers = new HashSet<char>();

            for (int i = 65; i <= 90; i++) {
                UpperLetters.Add((char)i);
            }
            for (int i = 97; i <= 122; i++) {
                LowerLetters.Add((char)i);
            }
            for (int i = 48; i <= 57; i++) {
                Numbers.Add((char)i);
            }
        }
        public static HashSet<char> UpperLetters {
            get;
            private set;
        }
        public static HashSet<char> LowerLetters {
            get;
            private set;
        }
        public static HashSet<char> Numbers {
            get;
            private set;
        }
    }
}
