using System.Collections.Generic;
using System.Linq;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class OnceParser<TToken, TResult> : IParser<TToken, List<TResult>> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _parser;

        public OnceParser(string name, IParser<TToken, TResult> parser)
        {
            Name = name;
            _parser = parser;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> state, IParserState<TToken> parserState)
        {
            var result = _parser.Parse(input, state, parserState.Call(_parser, input));

            if (result.WasSuccessful)
            {
                var values = new List<IUnionResultValue<TToken>>();

                foreach (var item in result.Values)
                {
                    values.Add(new UnionResultValue<TToken>(typeof(List<TResult>))
                    {
                        Value = new List<TResult>() { item.GetValue<TResult>() },
                        Reminder = item.Reminder,
                        ConsumedTokens = item.ConsumedTokens,
                        Position = item.Position,
                    });
                }
                return UnionResultFactory.Success<TToken, List<TResult>>(this, values);
            }
            else
            {
                return UnionResultFactory.Failure(this, $"Parser failed", input);
            }
        }
    }
}
