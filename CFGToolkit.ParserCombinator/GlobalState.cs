using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Parsers.Behaviors;

namespace CFGToolkit.ParserCombinator
{
    public class GlobalState<TToken> : IGlobalState<TToken> where TToken : IToken
    {
        public GlobalState()
        {
        }

        public Dictionary<IParser<TToken>, List<Action<AfterParseArgs<TToken>>>> AfterParseActions
        {
            get; set;
        }

        public int LastConsumedPosition { get; set; } = -1;

        public Stack<Frame<TToken>> LastConsumedCallStack { get; set; }

        public int LastFailedPosition { get; set; } = -1;

        public IParser<TToken> LastFailedParser { get; set; }
    }
}
