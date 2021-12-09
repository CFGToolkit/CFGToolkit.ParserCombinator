using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class SelectManyParser<TToken, T, U, V> : IParser<TToken, V> where TToken : IToken
    {
        private readonly IParser<TToken, T> _parser;
        private readonly Func<T, IParser<TToken, U>> _selector;
        private readonly Func<T, U, V> _projector;

        public SelectManyParser(IParser<TToken, T> parser, Func<T, IParser<TToken, U>> selector, Func<T, U, V> projector)
        {
            _parser = parser;
            _selector = selector;
            _projector = projector;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parseState)
        {
            var firstResult = _parser.Parse(input, globalState, parseState.Call(_parser, input));

            if (firstResult.WasSuccessful)
            {
                var values = new List<IUnionResultValue<TToken>>();
                var errorMessages = new List<string>();
                foreach (var item in firstResult.Values)
                {
                    var secondParser = _selector(item.GetValue<T>());
                    var secondParserResults = secondParser.Parse(item.Reminder, globalState, parseState.Call(secondParser, item.Reminder));

                    if (secondParserResults.WasSuccessful)
                    {
                        foreach (var secondItem in secondParserResults.Values)
                        {
                            values.Add(new UnionResultValue<TToken>(typeof(V))
                            {
                                Value = _projector(item.GetValue<T>(), secondItem.GetValue<U>()),
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
