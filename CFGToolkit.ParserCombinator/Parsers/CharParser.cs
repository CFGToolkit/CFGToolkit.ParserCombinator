using System;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class CharParser : IParser<CharToken, char>
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

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
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

            var @char = input.Source[input.Position + prefixLen].Value;
            return UnionResultFactory.Success(@char, input.Advance(total), this, position: input.Position, consumedTokens: total);
        }
    }
}
