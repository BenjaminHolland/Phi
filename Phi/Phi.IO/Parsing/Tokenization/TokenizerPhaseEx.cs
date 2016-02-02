using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Parsing.Tokenization {
    public abstract class TokenizerPhaseEx : TokenizerPhase {
        public const int Infinity = -1;
        private const string _invalidRangeMessage = "MaxLength cannot be less than MinLength";
        private const string _invalidValueMessage = "MaxLength and MinLength must be >=0 or TokenizerPhaseEx.Infinity";
        private void _throwIfInvalidRange() {
            if ((_minLength == Infinity ? Int32.MaxValue : _minLength) > (_maxLength == Infinity ? Int32.MaxValue : _maxLength)) {
                throw new InvalidOperationException(_invalidRangeMessage);
            }
        }
        private void _throwIfInvalidValue() {
            if (_minLength < -1 || _maxLength < -1) {
                throw new InvalidOperationException(_invalidValueMessage);
            }
        }
        private int _minLength;
        private int _maxLength;
        public int MaxLength {
            get {
                return _maxLength;
            }
            protected set {
                _maxLength = value;
                _throwIfInvalidRange();
                _throwIfInvalidValue();
            }
        }
        public int MinLength {
            get {
                return _minLength;
            }
            protected set {
                _minLength = value;
                _throwIfInvalidValue();
                _throwIfInvalidRange();
            }
        }
        public TokenizerPhaseEx(string phaseIdentifier, int minLength, int maxLength)
            : base(phaseIdentifier) {

            MaxLength = maxLength;
            MinLength = minLength;
        }


    }
}
