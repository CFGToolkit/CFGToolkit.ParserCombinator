using System;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class CharParser : BaseParser<CharToken, char>
    {
        private readonly Predicate<char> _predicate;
        private readonly string _description;
        private readonly bool _token;

        public CharParser(string name, Predicate<char> predicate, string description, bool token = false)
        {
            Name = name;
            _predicate = predicate;
            _description = description;
            _token = token;
        }

        public string Description => _description;

        protected override IUnionResult<CharToken> ParseInternal(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            if (input.AtEnd)
            {
                return UnionResultFactory.Failure(this, "Failed to match char. Unexpected end of input", input);
            }

            int prefixLen = 0;
            if (_token)
            {
                prefixLen = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), input.Position);
            }

            int i = 0;
            int strLength = input.AdvanceWhile(token => { return i++ < 1 && _predicate(token.Value); }, input.Position + prefixLen);

            if (strLength != 1)
            {
                return UnionResultFactory.Failure(this, "Failed to match char predicate", input);
            }

            int suffixLen = 0;
            if (_token)
            {
                suffixLen = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), input.Position + strLength + prefixLen);
            }

            int total = prefixLen + strLength + suffixLen;

            var @char = input.Tokens[input.Position + prefixLen].Value;
            return UnionResultFactory.Success(@char, input.Advance(total), this, position: input.Position, consumedTokens: total);
        }
    }
}
