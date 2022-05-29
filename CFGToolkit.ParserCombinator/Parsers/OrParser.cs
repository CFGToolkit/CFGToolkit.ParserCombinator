﻿using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class OrParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _left;
        private readonly IParser<TToken, TResult> _right;
        private readonly string _errorMessage;

        public OrParser(string name, IParser<TToken, TResult> left, IParser<TToken, TResult> right)
        {
            Name = name;
            _left = left;
            _right = right;
            _errorMessage = $"Both parsers failed: ({_left.Name}) and ({_right.Name})";
        }

        public string Name { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var firstResult = _left.Parse(input, globalState, parserCallStack.Call(_left, input));
            var secondResult = _right.Parse(input, globalState, parserCallStack.Call(_right, input));

            if (firstResult.WasSuccessful && secondResult.WasSuccessful)
            {
                firstResult.Values.AddRange(secondResult.Values);
                return UnionResultFactory.Success(this, firstResult);
            }
            if (firstResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, firstResult);
            }

            if (secondResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, secondResult);
            }

            return UnionResultFactory.Failure(this, _errorMessage, input);
        }
    }
}
