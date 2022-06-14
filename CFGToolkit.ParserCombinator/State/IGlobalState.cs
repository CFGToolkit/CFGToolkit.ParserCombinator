using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public interface IGlobalState<TToken> where TToken : IToken
    {
        int LastConsumedPosition { get; set; }

        int LastFailedPosition { get; set; }

        Lazy<List<Frame<TToken>>> LastConsumedCallStack { get; set; }

        ConcurrentBag<Lazy<List<Frame<TToken>>>> LastFailedCallStacks { get; set; }

        List<List<Frame<TToken>>> GetUniqueFailedCallStacks();

        Action<bool> UpdateHandler { get; set; }

        IUnionResult<TToken>[,] Cache { get; set; }
    }
}
