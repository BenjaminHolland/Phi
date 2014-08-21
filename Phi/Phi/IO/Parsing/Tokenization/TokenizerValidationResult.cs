using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Parsing.Tokenization {
    /// <summary>
    /// What action the Tokenizer should take with regards to the entirety of the data sentence
    /// </summary>
    public enum TokenizerSentenceAtion {
        /// <summary>
        /// The data sentence is a failure.
        /// </summary>
        Fail, 
        /// <summary>
        /// Continue processing the data sentence.
        /// </summary>
        Continue,
        /// <summary>
        /// The data sentence is complete.
        /// </summary>
        Complete,
        /// <summary>
        /// The tokenizer should discard the current tokenizer result, reset the phase counter, and continue processing.
        /// This can be used for data sentence formats that have a sentence identifier at the beginning.
        /// </summary>
        Reset,
    }

    /// <summary>
    /// The action the tokenizer should take with respect to the current phase.
    /// </summary>
    public enum TokenizerPhaseAction {
        
        /// <summary>
        /// Continue processing this phase.
        /// </summary>
        Continue,
        /// <summary>
        /// This phase is complete. Move to the next phase. 
        /// </summary>
        Complete,

        /// <summary>
        /// Discard the current phase content and jump to the phase specified by this phases SkipToPhaseID.
        /// </summary>
        JumpAndSkip,
        /// <summary>
        /// Complete this phase and jump to the phase specified by this phases' SkipToPhaseID;
        /// </summary>
        JumpAndComplete,
    }
    /// <summary>
    /// The action the tokenizer should take with regard to the current character
    /// </summary>
    public enum TokenizerCharacterAction {
        /// <summary>
        /// Add the current character to the current phases content string, advance the character index.
        /// </summary>
        Add,
        /// <summary>
        /// Advance the character index. Do not add the current character to the current phases content string.
        /// </summary>
        Ignore,
        /// <summary>
        /// Rewind the character index by one. 
        /// </summary>
        /// <remarks>
        /// Using this CharacterAction with TokenizerPhaseAction.Continue will result in an infinite loop.
        /// Tokenizers should throw an exception or fail if these are used together.
        /// </remarks>
        RewindOne,
    }

    /// <summary>
    /// The result of a TokenizerPhase.Validate() call. 
    /// </summary>
    /// 
    [Phi.Core.Development.TODO("Cache all different values of TokenizerValidatorResult and return those rather than making a new one each time.")]
    public class TokenizerValidationResult {
        TokenizerCharacterAction _characterAction;
        TokenizerPhaseAction _phaseAction;
        TokenizerSentenceAtion _sentenceAction;
        public static TokenizerValidationResult Create(TokenizerSentenceAtion sa, TokenizerPhaseAction pa,TokenizerCharacterAction ca){
            return new TokenizerValidationResult(sa,pa,ca);
        }
        public static TokenizerValidationResult CreateFailure() {
            return new TokenizerValidationResult(TokenizerSentenceAtion.Fail, TokenizerPhaseAction.Complete, TokenizerCharacterAction.Ignore);
        }
        private TokenizerValidationResult(TokenizerSentenceAtion sa, TokenizerPhaseAction pa,TokenizerCharacterAction ca) {
            _characterAction = ca;
            _phaseAction = pa;
            _sentenceAction = sa;

        }
        /// <summary>
        /// The action the tokenizer should take with respect to the current character
        /// </summary>
        public TokenizerCharacterAction CharacterAction {
            get {
                return _characterAction;
            }

        }
        /// <summary>
        /// The action the tokenizer should take with respect to the current phase
        /// </summary>
        public TokenizerPhaseAction PhaseAction {
            get {
                return _phaseAction;
            }

        }
        /// <summary>
        /// The action the tokenizer should take with respect to the enitre sentence
        /// </summary>
        public TokenizerSentenceAtion SentenceAction {
            get {
                return _sentenceAction;
            }

        }
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            TokenizerValidationResult r = obj as TokenizerValidationResult;
            if (r == null) {
                return false;
            }
            return r.CharacterAction == CharacterAction && r.PhaseAction == PhaseAction && r.SentenceAction == SentenceAction;
        }
        public bool Equals(TokenizerValidationResult r) {
            if (r == null) {
                return false;
            }
            return r.CharacterAction == CharacterAction && r.PhaseAction == PhaseAction && r.SentenceAction == SentenceAction;
        }
        public override int GetHashCode() {
            int hash=17;
            hash=(hash*13)+_sentenceAction.GetHashCode();
            hash=(hash*13)+_characterAction.GetHashCode();
            hash=(hash*13)+_phaseAction.GetHashCode();
            return hash;
        }
    }

}
