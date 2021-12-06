using CFGToolkit.ParserCombinator.Parsers.Behaviors;
using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator
{
    public interface IGlobalState<TToken> where TToken : IToken
    {
        Dictionary<IParser<TToken>, List<Action<AfterParseArgs<TToken>>>> AfterParseActions { get; set; }

        int LastConsumedPosition { get; set; }

        int LastFailedPosition { get; set; }

        Stack<Frame<TToken>> LastConsumedCallStack { get; set; }

        IParser<TToken> LastFailedParser { get; set; }
    }
}
