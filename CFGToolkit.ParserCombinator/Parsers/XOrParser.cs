namespace CFGToolkit.ParserCombinator.Parsers
{
    public class XOrParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> first;
        private readonly IParser<TToken, TResult> second;

        public XOrParser(string name, IParser<TToken, TResult> first, IParser<TToken, TResult> second)
        {
            Name = name;
            this.first = first;
            this.second = second;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var firstResult = first.Parse(input, globalState, parserState.Call(first, input));

            if (firstResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, firstResult);
            }

            var secondResult = second.Parse(input, globalState, parserState.Call(second, input));

            if (secondResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, secondResult);
            }
            else
            {
                return UnionResultFactory.Failure(this, $"Parser {Name} failed", input);
            }
        }
    }
}
