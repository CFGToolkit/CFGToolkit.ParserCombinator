﻿using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class SelectManyParser<TToken, T, U, V> : BaseParser<TToken, V> where TToken : IToken
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

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var firstResult = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (firstResult.IsSuccessful)
            {
                var values = new List<IUnionResultValue<TToken>>();
                foreach (var item in firstResult.Values)
                {
                    var secondParser = _selector(item.GetValue<T>());
                    var secondParserResults = secondParser.Parse(item.Reminder, globalState, parserCallStack.Call(secondParser, item.Reminder));

                    if (secondParserResults.IsSuccessful)
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
                return UnionResultFactory.Failure<TToken, V>(this, $"Parser {_parser.Name} failed in {Name} parser.", firstResult.MaxConsumed, input.Position);
            }
        }
    }
}
