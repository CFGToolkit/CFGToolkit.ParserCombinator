﻿using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class ExceptParser<TToken, T, TResult> : BaseParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _current;
        private readonly IParser<TToken, T> _except;

        public ExceptParser(string name, IParser<TToken, TResult> current, IParser<TToken, T> except)
        {
            Name = name;
            _current = current;
            _except = except;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var result = _except.Parse(input, globalState, parserCallStack.Call(_except, input));
            if (result.IsSuccessful)
            {
                return UnionResultFactory.Failure(this, "Unexpected success of parser: " + this, result.MaxConsumed, input.Position);
            }
            return _current.Parse(input, globalState, parserCallStack);
        }
    }
}
