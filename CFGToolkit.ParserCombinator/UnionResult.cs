using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator
{

    public class UnionResult<TToken> : IUnionResult<TToken> where TToken : IToken
    {
        public UnionResult(Type valueType)
        {
            ValueType = valueType;
        }

        public List<IUnionResultValue<TToken>> Values { get; set; }

        public Type ValueType
        {
            get;
            set;
        }

        public IParser<TToken> Parser { get; set; }

        public IGlobalState<TToken> State { get; set; }

        public bool WasSuccessful
        {
            get
            {

                if (Values == null || Values.Count == 0) return false;

                return true;
            }
        }

        public IInput<TToken> Input { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}
