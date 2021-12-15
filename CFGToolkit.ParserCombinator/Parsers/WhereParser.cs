using System;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class WhereParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _parser;
        private readonly Func<TResult, bool> _predicate;

        public WhereParser(string name, IParser<TToken, TResult> parser, Func<TResult, bool> predicate)
        {
            Name = name;
            _parser = parser;
            _predicate = predicate;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var result = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (result.WasSuccessful)
            {
                var filteredValues = result.Values.Where(i => _predicate(i.GetValue<TResult>()));

                if (filteredValues.Any())
                {
                    return UnionResultFactory.Success(this, filteredValues.ToList());
                }

                return UnionResultFactory.Failure(this, $"Parser {this.Name} failed", input);
            }

            return result;
        }
    }
}
