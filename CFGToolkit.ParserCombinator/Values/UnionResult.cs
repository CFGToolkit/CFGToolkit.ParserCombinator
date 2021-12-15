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

        public IInputStream<TToken> Input { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}
