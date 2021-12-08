using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class ThenFuncParser<TToken, T, U> : IParser<TToken, U> where TToken : IToken
    {
        private readonly IParser<TToken, T> first;
        private readonly Func<T, U> second;

        public ThenFuncParser(string name, IParser<TToken, T> first, Func<T, U> second)
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
                var values = new List<IUnionResultValue<TToken>>();

                foreach (var item in firstResult.Values)
                {
                    var value = second(item.GetValue<T>());
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
                return UnionResultFactory.Failure(this, $"Parser {first} failed", input);
            }
        }
    }
}
