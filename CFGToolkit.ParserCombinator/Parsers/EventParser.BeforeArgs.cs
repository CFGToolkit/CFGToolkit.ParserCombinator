using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class BeforeArgs<TToken> where TToken : IToken
    {
        public IInputStream<TToken> Input { get; set; }

        public IGlobalState<TToken> GlobalState { get; set; }

        public IParserCallStack<TToken> ParserCallStack { get; set; }
    }
}
