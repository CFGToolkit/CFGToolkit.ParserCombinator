using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class SelectManyParser<TToken, T, U, V> : IParser<TToken, V> where TToken : IToken
    {
        private readonly IParser<TToken, T> parser;
        private readonly Func<T, IParser<TToken, U>> selector;
        private readonly Func<T, U, V> projector;

        public SelectManyParser(IParser<TToken, T> parser, Func<T, IParser<TToken, U>> selector, Func<T, U, V> projector)
        {
            this.parser = parser;
            this.selector = selector;
            this.projector = projector;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parseState)
        {
            var firstResult = parser.Parse(input, globalState, parseState.Call(parser, input));

            if (firstResult.WasSuccessful)
            {
                var values = new List<IUnionResultValue<TToken>>();
                var errorMessages = new List<string>();
                foreach (var item in firstResult.Values)
                {
                    var secondParser = selector(item.GetValue<T>());
                    var secondParserResults = secondParser.Parse(item.Reminder, globalState, parseState.Call(secondParser, item.Reminder));

                    if (secondParserResults.WasSuccessful)
                    {
                        foreach (var secondItem in secondParserResults.Values)
                        {
                            values.Add(new UnionResultValue<TToken>(typeof(V))
                            {
                                Value = projector(item.GetValue<T>(), secondItem.GetValue<U>()),
                                Reminder = secondItem.Reminder,
                                ConsumedTokens = item.ConsumedTokens + secondItem.ConsumedTokens,
                                Position = item.Position,
                            });
                        }
                    }
                }

                return UnionResultFactory.Success<TToken, V>(this, values);
            }
            else
            {
                return UnionResultFactory.Failure<TToken, V>(this);
            }
        }
    }
}
