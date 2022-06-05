using System;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public class BeforeParseAction<TToken> where TToken : IToken
    {
        public int Index { get; set; }

        public Action<BeforeParseArgs<TToken>> Action {get;set;}
    }

    public class AfterParseAction<TToken> where TToken : IToken
    {
        public int Index { get; set; }

        public Action<AfterParseArgs<TToken>> Action { get; set; }
    }
}
