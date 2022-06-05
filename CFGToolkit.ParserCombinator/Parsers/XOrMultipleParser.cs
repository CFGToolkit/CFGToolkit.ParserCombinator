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
            foreach (var parser in _parsers)
            {
                var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));
                if (result.WasSuccessful)
                {
                    return UnionResultFactory.Success(this, result);
                }

            }

            return UnionResultFactory.Failure(this, "Parser failed", input);
        }
    }
}
