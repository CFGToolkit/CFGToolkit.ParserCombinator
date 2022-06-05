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

        List<Frame<TToken>> LastConsumedCallStack { get; set; }

        IParser<TToken> LastFailedParser { get; set; }

        Action<bool> UpdateHandler { get; set; }

        ConcurrentDictionary<long, IUnionResult<TToken>>[] Cache { get; set; }
    }
}
