using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class ThenFuncParser<TToken, T, U> : IParser<TToken, U> where TToken : IToken
    {
        private readonly IParser<TToken, T> _first;
        private readonly Func<T, U> _second;

        public ThenFuncParser(string name, IParser<TToken, T> first, Func<T, U> second)
        {
            Name = name;
            _first = first;
            _second = second;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var firstResult = _first.Parse(input, globalState, parserState.Call(_first, input));

            if (firstResult.WasSuccessful)
            {
                var values = new List<IUnionResultValue<TToken>>();

                foreach (var item in firstResult.Values)
                {
                    var value = _second(item.GetValue<T>());
                    values.Add(new UnionResultValue<TToken>(typeof(U))
                    {
                        Value = value,
                        Reminder = item.Reminder,
                        ConsumedTokens = item.ConsumedTokens,
                        Position = item.Position,
                    });
                }
                return UnionResultFactory.Success(this, values);
            }
            else
            {
                return UnionResultFactory.Failure(this, $"Parser {_first} failed", input);
            }
        }
    }
}
