using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class OrMultipleParser<TToken, TResult> : BaseParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult>[] _parsers;

        public OrMultipleParser(string name, IParser<TToken, TResult>[] parsers)
        {
            Name = name;
            _parsers = parsers;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            List<IUnionResultValue<TToken>> fullResults = null;
            foreach (var parser in _parsers)
            {
                var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));
                if (result.IsSuccessful)
                {
                    if (fullResults == null)
                    {
                        fullResults = result.Values;
                    }
                    else
                    {
                        fullResults.AddRange(result.Values);
                    }
                }
            }

            if (fullResults != null)
            {
                return UnionResultFactory.Success(this, fullResults);
            }

            return UnionResultFactory.Failure(this, "Parser failed", 0, input.Position);
        }
    }
}
