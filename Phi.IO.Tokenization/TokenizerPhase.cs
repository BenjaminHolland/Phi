using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Tokenization
{
    public abstract class TokenizerPhase {
        private const string _invalidPhaseIDMessage="PhaseID is invalid.";
        private void _throwIfPhaseIDInvalid(){
            if(_phaseID==null){
                throw new InvalidOperationException(_invalidPhaseIDMessage);
            }
        }
        private string _phaseID;
        public string PhaseID {
            get{
                return _phaseID;
            }
            protected set{
                _phaseID=value;
                _throwIfPhaseIDInvalid();
            }
        }
        public string JumpDestination{
            get;
            protected set;
        }
        public TokenizerPhase(string phaseIdentifier) {
            PhaseID=phaseIdentifier;
        }
        public abstract TokenizerValidationResult Validate(char c, string currentContent=null, TokenizerResult completedPhases = null);
    }
    
}
