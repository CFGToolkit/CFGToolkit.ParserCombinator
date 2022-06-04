using System;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers;

namespace CFGToolkit.ParserCombinator.State
{
    public class BeforeParseAction<TToken> where TToken : IToken
    {
        public int Index { get; set; }

        public Action<BeforeArgs<TToken>> Action {get;set;}
    }

    public class AfterParseAction<TToken> where TToken : IToken
    {
        public int Index { get; set; }

        public Action<AfterArgs<TToken>> Action { get; set; }
    }
}
