namespace CFGToolkit.ParserCombinator.Parsers
{
    public class BeforeArgs<TToken> where TToken : IToken
    {
        public IInput<TToken> Input { get; set; }

        public IGlobalState<TToken> GlobalState { get; set; }

        public IParserState<TToken> ParserState { get; set; }
    }
}
