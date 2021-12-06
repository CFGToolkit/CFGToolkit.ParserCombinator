namespace CFGToolkit.ParserCombinator.Parsers
{
    public class NotParser<TToken> : IParser<TToken, object> where TToken : IToken
    {
        private readonly IParser<TToken> parser;

        public NotParser(string name, IParser<TToken> parser)
        {
            Name = name;
            this.parser = parser;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var result = parser.Parse(input, globalState, parserState.Call(parser, input));

            if (result.WasSuccessful)
            {
                return UnionResultFactory.Failure(this, $"Parser {parser.Name} matched unexpectedly", input);
            }

            return UnionResultFactory.Success(null, input, this, input.Position, consumedTokens: 0);
        }
    }
}
