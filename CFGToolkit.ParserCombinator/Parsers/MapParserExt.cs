﻿using System;
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
            var firstResult = _first.Parse(input, globalState, parserCallStack);

            if (firstResult.IsSuccessful)
            {
                foreach (var item in firstResult.Values)
                {
                    var value = _second(_first, item);
                    item.Value = value;
                }
                return UnionResultFactory.Success(this, firstResult);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Parser first failed", firstResult.MaxConsumed, input.Position);
            }
        }
    }
}
