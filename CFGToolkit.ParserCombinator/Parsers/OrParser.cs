using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class OrParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _left;
        private readonly IParser<TToken, TResult> _right;

        public OrParser(string name, IParser<TToken, TResult> left, IParser<TToken, TResult> right)
        {
            Name = name;
            _left = left;
            _right = right;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> state, IParserState<TToken> parserState)
        {
            var firstResult = _left.Parse(input, state, parserState.Call(_left, input));
            var secondResult = _right.Parse(input, state, parserState.Call(_left, input));

            if (firstResult.WasSuccessful && secondResult.WasSuccessful)
            {
                var result = new List<IUnionResultValue<TToken>>(firstResult.Values);

                foreach (var item in secondResult.Values)
                {
                    result.Add(item);
                }

                return UnionResultFactory.Success(this, result);
            }
            if (firstResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, firstResult);
            }

            if (secondResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, secondResult);
            }

            return UnionResultFactory.Failure(this, $"Both parsers failed: ({_left.Name}) and ({_right.Name})", input);
        }
    }
}
