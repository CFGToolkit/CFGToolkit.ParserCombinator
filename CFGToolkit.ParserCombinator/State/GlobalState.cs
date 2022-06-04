using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public class GlobalState<TToken> : IGlobalState<TToken> where TToken : IToken
    {
        public GlobalState()
        {
        }

        public ConcurrentDictionary<string, ConcurrentBag<BeforeParseAction<TToken>>> BeforeParseActions
        {
            get; set;
        }

        public ConcurrentDictionary<string, ConcurrentBag<AfterParseAction<TToken>>> AfterParseActions
        {
            get; set;
        }

        public int LastConsumedPosition { get; set; } = -1;

        public List<Frame<TToken>> LastConsumedCallStack { get; set; }

        public ConcurrentDictionary<long, IUnionResult<TToken>>[] Cache { get; set; }

        public int LastFailedPosition { get; set; } = -1;

        public IParser<TToken> LastFailedParser { get; set; }

        public Action<bool> UpdateHandler { get; set; }
    }
}
