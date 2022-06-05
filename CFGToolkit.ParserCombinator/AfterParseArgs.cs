using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator
{
    public class AfterParseArgs<TToken> where TToken : IToken
    {
        public IUnionResult<TToken> ParserResult { get; set; }

        public IInputStream<TToken> Input { get; set; }

        public IGlobalState<TToken> GlobalState { get; set; }

        public IParserCallStack<TToken> ParserCallStack { get; set; }

        public bool Valid { get; set; } = true;
    }
}
