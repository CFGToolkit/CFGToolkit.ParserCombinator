using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class AfterArgs<TToken> where TToken : IToken
    {
        public IUnionResult<TToken> ParserResult { get; set; }

        public IInputStream<TToken> Input { get; set; }

        public IGlobalState<TToken> GlobalState { get; set; }

        public IParserCallStack<TToken> ParserCallStack { get; set; }

        public bool Valid { get; set; } = true;
    }
}
