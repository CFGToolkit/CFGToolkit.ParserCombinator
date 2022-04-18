using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class XOrParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _left;
        private readonly IParser<TToken, TResult> _right;

        public XOrParser(string name, IParser<TToken, TResult> left, IParser<TToken, TResult> right)
        {
            Name = name;
            _left = left;
            _right = right;
        }

        public string Name { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var firstResult = _left.Parse(input, globalState, parserCallStack.Call(_left, input));

            if (firstResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, firstResult);
            }

            var secondResult = _right.Parse(input, globalState, parserCallStack.Call(_right, input));

            if (secondResult.WasSuccessful)
            {
                return UnionResultFactory.Success(this, secondResult);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Parser failed", input);
            }
        }
    }
}
