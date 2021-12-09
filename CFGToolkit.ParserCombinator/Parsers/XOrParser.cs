namespace CFGToolkit.ParserCombinator.Parsers
{
    public class XOrParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _left;
        private readonly IParser<TToken, TResult> _right;

        public XOrParser(string name, IParser<TToken, TResult> left, IParser<TToken, TResult> right)
        {
            Name = name;
            _left = left;
            _right = right;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var firstResult = _left.Parse(input, globalState, parserState.Call(_left, input));

            if (firstResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, firstResult);
            }

            var secondResult = _right.Parse(input, globalState, parserState.Call(_right, input));

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
