namespace CFGToolkit.ParserCombinator.Parsers
{
    public class ExceptParser<TToken, T, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _current;
        private readonly IParser<TToken, T> _except;

        public ExceptParser(string name, IParser<TToken, TResult> current, IParser<TToken, T> except)
        {
            Name = name;
            _current = current;
            _except = except;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var result = _except.Parse(input, globalState, parserState.Call(_except, input));
            if (result.WasSuccessful)
            {
                return UnionResultFactory.Failure(this, "Unexpected success of parser: " + this, input);
            }
            return _current.Parse(input, globalState, parserState);
        }
    }
}
