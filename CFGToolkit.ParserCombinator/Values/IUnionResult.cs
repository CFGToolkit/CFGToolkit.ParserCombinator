﻿using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator.Values
{
    public interface IUnionResult<TToken> where TToken : IToken
    {
        public bool WasSuccessful { get; }

        IGlobalState<TToken> GlobalState { get; set; }

        List<IUnionResultValue<TToken>> Values { get; set; }

        IParser<TToken> Parser { get; set; }

        IInputStream<TToken> Input { get; set; }
    }
}
