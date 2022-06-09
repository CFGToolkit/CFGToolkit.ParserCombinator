using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator
{
    public interface IUnionResult<TToken> where TToken : IToken
    {
        public bool IsSuccessful { get; }

        IGlobalState<TToken> GlobalState { get; set; }

        List<IUnionResultValue<TToken>> Values { get; set; }

        IParser<TToken> Parser { get; set; }

        IInputStream<TToken> Input { get; set; }

        int MaxConsumed { get; }
    }
}
