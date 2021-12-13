namespace CFGToolkit.ParserCombinator.Parsers
{
    public class AfterParseArgs<TToken> where TToken : IToken
    {
        public IUnionResult<TToken> ParserResult { get; set; }

        public IInput<TToken> Input { get; set; }

        public IGlobalState<TToken> GlobalState { get; set; }

        public IParserState<TToken> ParserState { get; set; }

        public bool Valid { get; set; } = true;
    }
}
