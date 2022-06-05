using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class NotParser<TToken> : BaseParser<TToken, object> where TToken : IToken
    {
        private readonly IParser<TToken> _parser;

        public NotParser(string name, IParser<TToken> parser)
        {
            Name = name;
            _parser = parser;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var result = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (result.WasSuccessful)
            {
                return UnionResultFactory.Failure(this, "Parser matched unexpectedly", input);
            }

            return UnionResultFactory.Success(null, input, this, input.Position, consumedTokens: 0);
        }
    }
}
