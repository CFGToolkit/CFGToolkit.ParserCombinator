using System;
using System.Collections.Generic;
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
                return _isSuccessful && Values?.Count > 0;
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
                    int max = 0;
                    if (Values != null)
                    {
                        for (int i = 0; i < Values.Count; i++)
                        {
                            var consumed = Values[i].ConsumedTokens;
                            if (consumed > max) max = consumed;
                        }
                    }
                    _maxConsumed = max;
                }
                return _maxConsumed.Value;
            }
        }

        public IInputStream<TToken> Input { get; set; }

        public string ErrorMessage { get; internal set; }
    }
}
