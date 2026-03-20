using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class MapParserExt<TToken, T, U> : BaseParser<TToken, U> where TToken : IToken
    {
        private readonly IParser<TToken, T> _first;
        private readonly Func<IParser<TToken>, IUnionResultValue<TToken>, U> _second;

        public MapParserExt(string name, IParser<TToken, T> first, Func<IParser<TToken>, IUnionResultValue<TToken>, U> second)
        {
            Name = name;
            _first = first;
            _second = second;
            ShouldUpdateGlobalState = false;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var firstResult = _first.Parse(input, globalState, parserCallStack.Call(_first, input));

            if (firstResult.IsSuccessful)
            {
                var mappedValues = new List<IUnionResultValue<TToken>>(firstResult.Values.Count);
                foreach (var item in firstResult.Values)
                {
                    var value = _second(_first, item);
                    mappedValues.Add(new UnionResultValue<TToken>(typeof(U))
                    {
                        Value = value,
                        Reminder = item.Reminder,
                        Position = item.Position,
                        ConsumedTokens = item.ConsumedTokens,
                    });
                }
                return UnionResultFactory.Success(this, mappedValues);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Parser first failed", firstResult.MaxConsumed, input.Position);
            }
        }
    }
}
