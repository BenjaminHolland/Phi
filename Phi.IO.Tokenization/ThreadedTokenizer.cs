using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
namespace Phi.IO.Tokenization
{
    public class ThreadedTokenizer : IDisposable
    {
        private enum ThreadedTokenizerState
        {
            Processing,
            Succeded,
            Failed,
        }
        private const int FRAGMENT_WAIT_TIME = 50;
        private const int LOOP_STARTUP_TIME = 50;
        private long _stop;
        private Thread _tokenizerThread;
        private BlockingCollection<string> _fragments;
        private void _tokenizerLoop()
        {
            System.Diagnostics.Debug.WriteLine("Started _tokenizerThread");
            ThreadedTokenizerState currentState = ThreadedTokenizerState.Processing;
            string currentFragment = "";
            TokenizerResult currentResult = new TokenizerResult();
            StringBuilder currentContent = new StringBuilder();
            TokenizerValidationResult pResult;
            int cIdx = 0;
            int pIdx = 0;

            while (Interlocked.Read(ref _stop) <= 0)
            {
                while (!_fragments.TryTake(out currentFragment, FRAGMENT_WAIT_TIME))
                {
                    if (Interlocked.Read(ref _stop) > 0)
                    {
                        break;
                    }
                }
                if (currentFragment != null)
                {
                    for (cIdx = 0; (cIdx < currentFragment.Length); cIdx++)
                    {
                        pResult = _phases[pIdx].Validate(currentFragment[cIdx], currentContent.ToString(), currentResult);
                        switch (pResult.SentenceAction)
                        {
                            case TokenizerSentenceAtion.Fail:
                                currentResult.SetAsFailure("Phase Character Failure");
                                currentState = ThreadedTokenizerState.Failed;
                                break;
                            case TokenizerSentenceAtion.Complete:
                                currentContent.Append(currentFragment[cIdx]);
                                currentResult.AddCompletedPhase(_phases[pIdx].PhaseID, currentContent.ToString());
                                currentState = ThreadedTokenizerState.Succeded;
                                break;
                            case TokenizerSentenceAtion.Continue:
                                switch (pResult.PhaseAction)
                                {
                                    case TokenizerPhaseAction.Complete:
                                        switch (pResult.CharacterAction)
                                        {
                                            case TokenizerCharacterAction.Add:
                                                currentContent.Append(currentFragment[cIdx]);
                                                break;
                                            case TokenizerCharacterAction.Ignore:
                                                break;
                                            case TokenizerCharacterAction.RewindOne:
                                                currentResult.SetAsFailure("Infinite Loop");
                                                currentState = ThreadedTokenizerState.Failed;
                                                break;
                                        }
                                        if (currentState != ThreadedTokenizerState.Failed)
                                        {
                                            currentResult.AddCompletedPhase(_phases[pIdx].PhaseID, currentContent.ToString());
                                            currentContent.Clear();
                                            if (pIdx++ >= _phases.Count)
                                            {
                                                currentResult.SetAsFailure("No More Phases");
                                                currentState = ThreadedTokenizerState.Failed;
                                            }
                                        }
                                        break;
                                    case TokenizerPhaseAction.Continue:
                                        switch (pResult.CharacterAction)
                                        {
                                            case TokenizerCharacterAction.Add:
                                                currentContent.Append(currentFragment[cIdx]);
                                                break;
                                            case TokenizerCharacterAction.Ignore:
                                                break;
                                            case TokenizerCharacterAction.RewindOne:
                                                currentResult.SetAsFailure("Infinite Loop");
                                                currentState = ThreadedTokenizerState.Failed;
                                                break;
                                        }
                                        break;
                                    case TokenizerPhaseAction.JumpAndComplete:
                                        switch (pResult.CharacterAction)
                                        {
                                            case TokenizerCharacterAction.Add:
                                                currentContent.Append(currentFragment[cIdx]);
                                                break;
                                            case TokenizerCharacterAction.Ignore:
                                                break;
                                            case TokenizerCharacterAction.RewindOne:
                                                cIdx--;
                                                break;
                                        }
                                        currentResult.AddCompletedPhase(_phases[pIdx].PhaseID, currentContent.ToString());
                                        if (_phases[pIdx].JumpDestination == null)
                                        {
                                            currentResult.SetAsFailure("Jump Failure: Jump Destination \"null\"");
                                            currentState = ThreadedTokenizerState.Failed;
                                        }
                                        if (currentState != ThreadedTokenizerState.Failed)
                                        {
                                            currentContent.Clear();
                                            TokenizerPhase jumpPhase = null;
                                            if ((jumpPhase = _phases.Find(x => x.PhaseID == _phases[pIdx].JumpDestination)) != null)
                                            {
                                                pIdx = _phases.IndexOf(jumpPhase);
                                            }
                                            else
                                            {
                                                currentResult.SetAsFailure(String.Format("Jump Failure: Phase \"{0}\" Not Found", _phases[pIdx].JumpDestination));
                                                currentState = ThreadedTokenizerState.Failed;
                                            }
                                        }
                                        break;
                                    case TokenizerPhaseAction.JumpAndSkip:
                                        switch (pResult.CharacterAction)
                                        {
                                            case TokenizerCharacterAction.Add:
                                                currentResult.SetAsFailure("Invalid State");
                                                currentState = ThreadedTokenizerState.Failed;
                                                break;
                                            case TokenizerCharacterAction.Ignore:
                                                break;
                                            case TokenizerCharacterAction.RewindOne:
                                                cIdx--;
                                                break;
                                        }
                                        if (currentState != ThreadedTokenizerState.Failed)
                                        {
                                            currentContent.Clear();
                                            TokenizerPhase jumpPhase = null;
                                            if ((jumpPhase = _phases.Find(x => x.PhaseID == _phases[pIdx].JumpDestination)) != null)
                                            {
                                                pIdx = _phases.IndexOf(jumpPhase);
                                            }
                                            else
                                            {
                                                currentResult.SetAsFailure(String.Format("Jump Failure: Phase \"{0}\" Not Found", _phases[pIdx].JumpDestination));
                                                currentState = ThreadedTokenizerState.Failed;
                                            }
                                        }
                                        break;
                                }
                                break;
                            case TokenizerSentenceAtion.Reset:
                                switch (pResult.CharacterAction)
                                {
                                    case TokenizerCharacterAction.Add:
                                        currentResult.SetAsFailure("Invalid State");
                                        currentState = ThreadedTokenizerState.Failed;
                                        break;
                                    case TokenizerCharacterAction.Ignore:
                                        break;
                                    case TokenizerCharacterAction.RewindOne:
                                        cIdx--;
                                        break;
                                }
                                if (currentState != ThreadedTokenizerState.Failed)
                                {
                                    currentResult.SetAsFailure("Sentence Reset");
                                    currentState = ThreadedTokenizerState.Failed;
                                }
                                break;
                        }
                        switch (currentState)
                        {
                            case ThreadedTokenizerState.Processing:
                                break;
                            case ThreadedTokenizerState.Failed:
                                FailureAction(currentResult);
                                currentResult = new TokenizerResult();
                                pIdx = 0;
                                currentContent.Clear();
                                currentState = ThreadedTokenizerState.Processing;
                                break;
                            case ThreadedTokenizerState.Succeded:
                                SuccessAction(currentResult);
                                currentResult = new TokenizerResult();
                                pIdx = 0;
                                currentContent.Clear();
                                currentState = ThreadedTokenizerState.Processing;
                                break;
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("Ending _tokenizerThread");
        }
        public ThreadedTokenizer(IEnumerable<TokenizerPhase> phases)
        {
            if (phases.Count() <= 0)
            {
                throw new ArgumentException("phases cannot be empty");
            }
            this._phases = new List<TokenizerPhase>();
            this._phases.AddRange(phases);
            _stop = 0;
            _tokenizerThread = new Thread(_tokenizerLoop);
            _tokenizerThread.Name = "_tokenizerThread";
            _tokenizerThread.SetApartmentState(ApartmentState.STA);
            _fragments = new BlockingCollection<string>(new ConcurrentQueue<string>());
            _tokenizerThread.Start();
            Thread.Sleep(LOOP_STARTUP_TIME);//Wait for thread to start up.
        }
        private List<TokenizerPhase> _phases;
        public void TokenizeFragment(string fragment)
        {
            _fragments.Add(fragment);
        }
        public Action<TokenizerResult> SuccessAction;
        public Action<TokenizerResult> FailureAction;
        #region Disposal
        public void Dispose()
        {
            Interlocked.Increment(ref _stop);

            _tokenizerThread.Join();

            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
