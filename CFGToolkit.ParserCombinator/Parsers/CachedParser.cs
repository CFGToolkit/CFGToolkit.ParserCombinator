using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class CachedParser<TToken, T> : BaseParser<TToken, T> where TToken : IToken
    {
        private readonly IParser<TToken, T> _parser;

        public CachedParser(string name, IParser<TToken, T> parser, long id)
        {
            Name = name;
            _parser = parser;
            Id = id;
            ShouldUpdateGlobalState = false;
        }

        public long Id { get; set; }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            if (globalState.Cache != null)
            {
                var val = globalState.Cache[input.Position, Id];
                if (val != null)
                {
                    return val;
                }

                var newResult = _parser.Parse(input, globalState, parserCallStack);
                globalState.Cache[input.Position, Id] = newResult;
                return newResult;
            }
            else
            {
                return _parser.Parse(input, globalState, parserCallStack);
            }
        }
    }
}
