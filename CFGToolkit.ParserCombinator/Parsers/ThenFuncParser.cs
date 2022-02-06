using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

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

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var firstResult = _first.Parse(input, globalState, parserCallStack.Call(_first, input));

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
                return UnionResultFactory.Failure(this, "Parser first failed", input);
            }
        }
    }
}
