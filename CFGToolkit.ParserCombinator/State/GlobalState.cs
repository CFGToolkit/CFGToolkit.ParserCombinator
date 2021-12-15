using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers;

namespace CFGToolkit.ParserCombinator.State
{
    public class GlobalState<TToken> : IGlobalState<TToken> where TToken : IToken
    {
        public GlobalState()
        {
        }

        public Dictionary<IParser<TToken>, List<Action<BeforeArgs<TToken>>>> BeforeParseActions
        {
            get; set;
        }

        public Dictionary<IParser<TToken>, List<Action<AfterArgs<TToken>>>> AfterParseActions
        {
            get; set;
        }

        public int LastConsumedPosition { get; set; } = -1;

        public Stack<Frame<TToken>> LastConsumedCallStack { get; set; }

        public int LastFailedPosition { get; set; } = -1;

        public IParser<TToken> LastFailedParser { get; set; }
    }
}
