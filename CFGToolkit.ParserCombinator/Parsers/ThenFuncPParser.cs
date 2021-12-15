using System;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class ThenFuncPParser<TToken, T, U> : IParser<TToken, U> where TToken : IToken
    {
        private readonly IParser<TToken, T> _first;
        private readonly Func<T, IParser<TToken, U>> _second;

        public ThenFuncPParser(string name, IParser<TToken, T> first, Func<T, IParser<TToken, U>> second)
        {
            Name = name;
            _first = first;
            _second = second;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var firstResult = _first.Parse(input, globalState, parserCallStack.Call(_first, input));

            if (firstResult.WasSuccessful)
            {
                IParser<TToken, U> secondParser = null;

                var values = new List<IUnionResultValue<TToken>>();
                foreach (var item in firstResult.Values)
                {
                    secondParser = _second(item.GetValue<T>());

                    var tmp = secondParser.Parse(item.Reminder, globalState, parserCallStack);

                    if (tmp.WasSuccessful)
                    {
                        foreach (var secondItem in tmp.Values)
                        {
                            secondItem.Position = item.Position;
                            secondItem.ConsumedTokens = item.ConsumedTokens + secondItem.ConsumedTokens;
                            values.Add(secondItem);
                        }
                    }
                }

                if (values.Any())
                {
                    return UnionResultFactory.Success(this, values);
                }
                else
                {
                    return UnionResultFactory.Failure(this, $"Parser {secondParser?.Name} failed in {Name} parser", input);
                }
            }
            else
            {
                return UnionResultFactory.Failure(this, $"Parser {_first} failed in {Name} parser", input);
            }
        }
    }
}
