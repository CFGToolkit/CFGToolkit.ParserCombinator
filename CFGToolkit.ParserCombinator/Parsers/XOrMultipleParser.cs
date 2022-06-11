using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class XOrMultipleParser<TToken, TResult> : BaseParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult>[] _parsers;

        public XOrMultipleParser(string name, IParser<TToken, TResult>[] parsers)
        {
            Name = name;
            _parsers = parsers;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            int max = 0;
            foreach (var parser in _parsers)
            {
                var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));

                if (result.IsSuccessful)
                {
                    return UnionResultFactory.Success(this, result);
                }
                else
                {
                    max = Options.FullErrorReporting ? Math.Max(max, result.MaxConsumed) : 0;
                }
            }

            return UnionResultFactory.Failure(this, "Parser failed", max, input.Position);
        }
    }

    public class XOrMultipleParser<TResult> : BaseParser<CharToken, TResult>
    {
        private readonly bool _firstMode;
        private readonly IParser<CharToken, TResult>[] _parsers;
        private readonly Lazy<List<(int, string)>> _firstSet;

        public XOrMultipleParser(string name, bool firstMode, IParser<CharToken, TResult>[] parsers)
        {
            Name = name;
            _firstMode = firstMode;
            _parsers = parsers;
            if (firstMode)
            {
                _firstSet = new Lazy<List<(int, string)>>(() => Tags.Where(t => t.Key.StartsWith("First:")).Select(s => (int.Parse(s.Key.Substring(6)), s.Value)).OrderByDescending(s => s.Value.Length).ToList());
            }
        }

        protected override IUnionResult<CharToken> ParseInternal(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            if (_firstMode)
            {
                foreach (var value in _firstSet.Value)
                {
                    if (input.StartsWith(value.Item2))
                    {
                        var parser = _parsers[value.Item1];
                        return parser.Parse(input, globalState, parserCallStack.Call(parser, input));
                    }
                }
            }

            int max = 0;
            foreach (var parser in _parsers)
            {
                var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));

                if (result.IsSuccessful)
                {
                    return UnionResultFactory.Success(this, result);
                }
                else
                {
                    max = Options.FullErrorReporting ? Math.Max(max, result.MaxConsumed) : 0;
                }
            }

            return UnionResultFactory.Failure(this, "Parser failed", max, input.Position);
        }
    }
}
