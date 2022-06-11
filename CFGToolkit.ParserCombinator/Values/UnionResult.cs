using System;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator.Values
{

    public class UnionResult<TToken> : IUnionResult<TToken> where TToken : IToken
    {
        public UnionResult(Type valueType)
        {
            ValueType = valueType;
        }

        private List<IUnionResultValue<TToken>> _values;

        public List<IUnionResultValue<TToken>> Values
        {
            get
            {
                return _values;
            }

            set
            {
                _values = value;
                _maxConsumed = null;
            }
        }

        public Type ValueType
        {
            get;
            set;
        }

        public IParser<TToken> Parser { get; set; }

        public IGlobalState<TToken> GlobalState { get; set; }


        public bool _isSuccessful = true;

        public bool IsSuccessful
        {
            get
            {
                return Values?.Count > 0 && _isSuccessful;
            }
            set
            {
                _isSuccessful = value;
            }
        }


        private int? _maxConsumed = null;

        public int MaxConsumed
        {
            get
            {
                if (!Options.FullErrorReporting)
                {
                    return 0;
                }

                if (_maxConsumed == null)
                {
                    _maxConsumed = Values != null && Values.Count > 0 ? Values.Max(v => v.ConsumedTokens) : 0;
                }
                return _maxConsumed.Value;
            }
        }

        public IInputStream<TToken> Input { get; set; }

        public string ErrorMessage { get; internal set; }
    }
}
