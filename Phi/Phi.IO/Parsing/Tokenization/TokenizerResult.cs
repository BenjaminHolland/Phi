using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Parsing.Tokenization {
    public class TokenizerResult {
        public string FailureMessage {
            get;
            private set;
        }
        private const string duplicatePhaseIDMessage = "result already contains a phase with this phaseID";
        protected Dictionary<string, string> completePhases;
        public TokenizerResult() {
            completePhases = new Dictionary<string, string>();
            IsFailed = false;
        }
        public void Reset() {
            completePhases.Clear();
        }
        public Boolean IsFailed {
            get;
            private set;
        }
        public void AddCompletedPhase(string phaseID, string phaseContent) {
            if (!completePhases.ContainsKey(phaseID)) {
                completePhases.Add(phaseID, phaseContent);
            }
            else {
                throw new InvalidOperationException(duplicatePhaseIDMessage);
            }
        }
        public string this[string phaseID] {
            get {
                if (completePhases.ContainsKey(phaseID)) {
                    return completePhases[phaseID];
                }
                else {
                    return null;
                }
            }
        }
        public void SetAsFailure(string message) {
            IsFailed = true;
            FailureMessage = message;
        }
        public void CopyTo(TokenizerResult other) {
            foreach (var kvpair in completePhases) {
                other.AddCompletedPhase(kvpair.Key, kvpair.Value);
            }
            if (IsFailed) {
                other.SetAsFailure(FailureMessage);
            }
        }
    }
}
