using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class OrParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> first;
        private readonly IParser<TToken, TResult> second;

        public OrParser(string name, IParser<TToken, TResult> first, IParser<TToken, TResult> second)
        {
            Name = name;
            this.first = first;
            this.second = second;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> state, IParserState<TToken> parserState)
        {
            var firstResult = first.Parse(input, state, parserState.Call(first, input));
            var secondResult = second.Parse(input, state, parserState.Call(first, input));

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

            var resultFailures = new List<IUnionResultValue<TToken>>();
            resultFailures.Add(new UnionResultValue<TToken>(typeof(TResult))
            {
                Reminder = input,
                ErrorMessage = $"Both parsers failed: ({first.Name}) and ({second.Name})",
                Position = input.Position,
                Value = default(TResult),
                WasSuccessful = false
            });

            return UnionResultFactory.Failure(this, resultFailures);
        }
    }
}
