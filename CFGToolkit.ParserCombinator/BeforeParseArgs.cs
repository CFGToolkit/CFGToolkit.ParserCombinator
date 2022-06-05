using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator
{
    public class BeforeParseArgs<TToken> where TToken : IToken
    {
        public IInputStream<TToken> Input { get; set; }

        public IGlobalState<TToken> GlobalState { get; set; }

        public IParserCallStack<TToken> ParserCallStack { get; set; }

        public bool Skip { get; set; } = false;
    }
}
