using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Parsing.Tokenization {
  
    /// <summary>
    /// Standard Tokenizer. Takes a string and seperates it into a number of smaller, named strings, based on the current phase list.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public class StandardTokenizer {
        private List<TokenizerPhase> phases;
        public StandardTokenizer()
            {
                phases = new List<TokenizerPhase>();
        }
        /// <summary>
        /// Add a phase to the phase list.
        /// </summary>
        /// <param name="phase">The phase to add to the list. This phases' phaseID must be unique within the list</param>
        public void AddPhase(TokenizerPhase phase) {
            if (!phases.Exists(p => p.PhaseID == phase.PhaseID)) {
                phases.Add(phase);
            }
        }
        /// <summary>
        /// Removes all phases with the specified phaseID
        /// </summary>
        /// <param name="phase"></param>
        public void RemovePhase(string phaseID) {
            phases.RemoveAll(p => p.PhaseID == phaseID);
        }
        /// <summary>
        /// Tokenize data sentence based on the current phase list
        /// </summary>
        /// <param name="sentence">The sentence to tokenize</param>
        /// <returns>A tokenizer result that contains the results of the tokenization</returns>
        public TokenizerResult TokenizeSentence(string sentence) {
            TokenizerResult result = new TokenizerResult();
            int cIdx = 0;
            int pIdx = 0;
            StringBuilder currentContent = new StringBuilder();
            TokenizerValidationResult pResult;
            bool failed = false;
            bool succeded = false;
            for (cIdx = 0; (cIdx < sentence.Length) && (!failed)&&(!succeded); cIdx++) {
                pResult = phases[pIdx].Validate(sentence[cIdx], currentContent.ToString(), result);
                switch (pResult.SentenceAction) {
                    case TokenizerSentenceAtion.Fail:
                        result.SetAsFailure("Phase Character Failure");
                        failed = true;
                        break;
                    case TokenizerSentenceAtion.Complete:
                        currentContent.Append(sentence[cIdx]);
                        result.AddCompletedPhase(phases[pIdx].PhaseID, currentContent.ToString());
                        succeded = true;
                        break;
                    case TokenizerSentenceAtion.Continue:
                        switch (pResult.PhaseAction) {
                            case TokenizerPhaseAction.Complete:
                                switch (pResult.CharacterAction) {
                                    case TokenizerCharacterAction.Add:
                                        currentContent.Append(sentence[cIdx]);
                                        break;
                                    case TokenizerCharacterAction.Ignore:
                                        break;
                                    case TokenizerCharacterAction.RewindOne:
                                        result.SetAsFailure("Infinite Loop");
                                        failed = true;
                                        break;
                                }
                                if (!failed) {
                                    result.AddCompletedPhase(phases[pIdx].PhaseID, currentContent.ToString());
                                    currentContent.Clear();
                                    if (pIdx++ >= phases.Count) {
                                        result.SetAsFailure("No More Phases");
                                        failed = true;
                                    }
                                }
                                break;
                            case TokenizerPhaseAction.Continue:
                                switch (pResult.CharacterAction) {
                                    case TokenizerCharacterAction.Add:
                                        currentContent.Append(sentence[cIdx]);
                                        break;
                                    case TokenizerCharacterAction.Ignore:
                                        break;
                                    case TokenizerCharacterAction.RewindOne:
                                        result.SetAsFailure("Infinite Loop");
                                        failed = true;
                                        break;
                                }
                                break;
                            case TokenizerPhaseAction.JumpAndComplete:
                                switch (pResult.CharacterAction) {
                                    case TokenizerCharacterAction.Add:
                                        currentContent.Append(sentence[cIdx]);
                                        break;
                                    case TokenizerCharacterAction.Ignore:
                                        break;
                                    case TokenizerCharacterAction.RewindOne:
                                        cIdx--;
                                        break;
                                }
                                result.AddCompletedPhase(phases[pIdx].PhaseID, currentContent.ToString());
                                if (phases[pIdx].JumpDestination == null) {
                                    result.SetAsFailure("Jump Failure: Jump Destination \"null\"");
                                    failed = true;
                                }
                                if (!failed) {
                                    currentContent.Clear();
                                    TokenizerPhase jumpPhase = null;
                                    if ((jumpPhase = phases.Find(x => x.PhaseID == phases[pIdx].JumpDestination)) != null) {
                                        pIdx = phases.IndexOf(jumpPhase);
                                    }
                                    else {
                                        result.SetAsFailure(String.Format("Jump Failure: Phase \"{0}\" Not Found", phases[pIdx].JumpDestination));
                                        failed = true;
                                    }
                                }
                                break;
                            case TokenizerPhaseAction.JumpAndSkip:
                                switch (pResult.CharacterAction) {
                                    case TokenizerCharacterAction.Add:
                                        result.SetAsFailure("Invalid State");
                                        failed = true;
                                        break;
                                    case TokenizerCharacterAction.Ignore:
                                        break;
                                    case TokenizerCharacterAction.RewindOne:
                                        cIdx--;
                                        break;
                                }
                                if (!failed) {
                                    currentContent.Clear();
                                    TokenizerPhase jumpPhase = null;
                                    if ((jumpPhase = phases.Find(x => x.PhaseID == phases[pIdx].JumpDestination)) != null) {
                                        pIdx = phases.IndexOf(jumpPhase);
                                    }
                                    else {
                                        result.SetAsFailure(String.Format("Jump Failure: Phase \"{0}\" Not Found", phases[pIdx].JumpDestination));
                                        failed = true;
                                    }
                                }
                                break;
                        }
                        break;
                    case TokenizerSentenceAtion.Reset:
                        switch (pResult.CharacterAction) {
                            case TokenizerCharacterAction.Add:
                                result.SetAsFailure("Invalid State");
                                failed = true;
                                break;
                            case TokenizerCharacterAction.Ignore:
                                break;
                            case TokenizerCharacterAction.RewindOne:
                                cIdx--;
                                break;
                        }
                        if (!failed) {
                            result.SetAsFailure("Sentence Reset");
                            failed = true;
                        }
                        break;
                }
            }
            if (!succeded) {
                result.SetAsFailure("Not Enough Characters");
            }
            return result;
        }
    }
    
}
