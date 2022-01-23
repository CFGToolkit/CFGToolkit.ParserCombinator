using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class StringParser : IParser<CharToken, string>
    {
        private readonly string _string;
        private readonly bool _token;

        public StringParser(string name, string @string, bool token = false)
        {
            Name = name;
            _string = @string;
            _token = token;
        }

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            int prefixLen = 0;
            if (_token)
            {
               prefixLen = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), input.Position);
            }

            int i = 0;
            int strLength = input.AdvanceWhile(token => { return i < _string.Length && token.Value == _string[i++]; }, input.Position + prefixLen);

            if (strLength != _string.Length)
            {
                return UnionResultFactory.Failure(this, "Failed to match string: " + _string, input);
            }

            int suffixLen = 0;
            if (_token)
            {
               suffixLen = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), input.Position + strLength + prefixLen);
            }

            int total = prefixLen + strLength + suffixLen;

            return UnionResultFactory.Success(_string, input.Advance(total), this, position: input.Position, consumedTokens: total);
        }
    }
}
