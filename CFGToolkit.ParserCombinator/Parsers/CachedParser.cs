﻿using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class CachedParser<TToken, T> : IParser<TToken, T> where TToken : IToken
    {
        private readonly IParser<TToken, T> _parser;

        public CachedParser(string name, IParser<TToken, T> parser, long id)
        {
            Name = name;
            _parser = parser;
            Id = id;
        }

        public string Name { get; set; }

        public long Id { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            if (input.AtEnd)
            {
                return UnionResultFactory.Failure(this, "Unexpected end of input", input);
            }

            if (globalState.Cache[input.Position].TryGetValue(Id, out var result))
            {
                return result;
            }

            var newResult = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (newResult.WasSuccessful)
            {
                globalState.Cache[input.Position].Add(Id, newResult);
            }

            return newResult;
        }
    }
}