using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phi.IO.Parsing.Tokenization;
namespace Phi.IO {
    [Phi.Core.Development.TODO("Test Class")]
    public static class PhiSentencePhases {
        public static readonly TokenizerPhase StartPhase=new PhiStartPhase();
        public static TokenizerPhase GetDataPhase(int dataIdx){
            return new PhiDataPhase(dataIdx);
        }
        public static readonly TokenizerPhase EndPhase=new PhiEndPhase();
        public static List<TokenizerPhase> GetSentenceFormat(int dataPhaseCount){
            List<TokenizerPhase> ret = new List<TokenizerPhase>();
            ret.Add(StartPhase);
            for (int i = 0; i < dataPhaseCount; i++) {
                ret.Add(GetDataPhase(i));
            }
            ret.Add(EndPhase);
            return ret;
        }
    }
    [Phi.Core.Development.TODO("Test Class")]
    
    public class PhiStartPhase:TokenizerPhaseEx{
        internal TokenizerValidationResult CompleteResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Continue, TokenizerPhaseAction.Complete, TokenizerCharacterAction.Add);
        internal TokenizerValidationResult FailureResult = TokenizerValidationResult.CreateFailure();
        public PhiStartPhase()
            : base("Start", 1, 1) {
        }
        public override TokenizerValidationResult Validate(char c, string currentContent = null, TokenizerResult completedPhases = null) {
            TokenizerValidationResult result = FailureResult;
            if (c == '~'&&currentContent == "") {
                result=CompleteResult;
            }
            return result;
        }
    }
    [Phi.Core.Development.TODO("Test Class")]
    public class PhiDataPhase : TokenizerPhaseEx {
        internal TokenizerValidationResult CompleteResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Continue, TokenizerPhaseAction.Complete, TokenizerCharacterAction.Ignore);
        internal TokenizerValidationResult ResetResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Reset, TokenizerPhaseAction.Continue, TokenizerCharacterAction.RewindOne);
        internal TokenizerValidationResult JumpToEndResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Continue, TokenizerPhaseAction.JumpAndComplete, TokenizerCharacterAction.RewindOne);
        internal TokenizerValidationResult FailureResult = TokenizerValidationResult.CreateFailure();
        internal TokenizerValidationResult ContinueResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Continue, TokenizerPhaseAction.Continue, TokenizerCharacterAction.Add);
        public PhiDataPhase(int dataIdx):base(String.Format("Data{0}",dataIdx),1,1024) {
        }
        public override TokenizerValidationResult Validate(char c, string currentContent = null, TokenizerResult completedPhases = null) {
            TokenizerValidationResult result=FailureResult;
            bool ending=false;
            if (c == '~') {
                result= ResetResult;
            }
            else if (c=='\r'){
                JumpDestination="End";
                result=JumpToEndResult;
                ending=true;
            }else if(c==','){
                if (currentContent.Length < MinLength) {
                    result = FailureResult;
                }
                else {
                    result = CompleteResult;
                    ending = true;
                }
            }
            else {
                result = ContinueResult;
            }
            if (currentContent.Length + 1 > MaxLength) {
                if(!ending){
                    result=FailureResult;
                }
            }
            return result;
        }
    }
    [Phi.Core.Development.TODO("Test Class")]
    public class PhiEndPhase : TokenizerPhaseEx {
        internal TokenizerValidationResult CompleteResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Complete, TokenizerPhaseAction.Complete, TokenizerCharacterAction.Add);
        internal TokenizerValidationResult ResetResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Reset, TokenizerPhaseAction.Continue, TokenizerCharacterAction.RewindOne);
        internal TokenizerValidationResult ContinueResult = TokenizerValidationResult.Create(TokenizerSentenceAtion.Continue, TokenizerPhaseAction.Continue, TokenizerCharacterAction.Add);
        internal TokenizerValidationResult FailureResult = TokenizerValidationResult.CreateFailure();
        public PhiEndPhase():base("End",2,2) {
            
        }
        public override TokenizerValidationResult Validate(char c, string currentContent = null, TokenizerResult completedPhases = null) {
            TokenizerValidationResult result = FailureResult;
            if (c == '~') {
                result = ResetResult;
            }
            else if (c == '\r' && currentContent == "") {
                result = ContinueResult;
            }
            else if (c == '\n' && currentContent == "\r") {
                result = CompleteResult;
            }
            if (currentContent.Length + 1 >= MaxLength) {
                result = FailureResult;
            }
            return result;
            
        }
    }
}
