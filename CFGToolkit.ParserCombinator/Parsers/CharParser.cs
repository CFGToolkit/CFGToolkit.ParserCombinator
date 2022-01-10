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
            var current = input;

            if (_token)
            {
                current = current.AdvanceWhile(token => char.IsWhiteSpace(token.Value), true);
            }

            char c = '0';
            if (!current.AtEnd)
            {
                c = current.Current.Value;
                if (!_predicate(c))
                {
                    return UnionResultFactory.Failure(this, Name + " failed.", input);
                }
                current = current.Advance();
            }
            else
            {
                return UnionResultFactory.Failure(this, Name + " failed. End of input unexpected.", input);
            }

            if (_token)
            {
                current = current.AdvanceWhile(token => char.IsWhiteSpace(token.Value), false);
            }

            return UnionResultFactory.Success(c, current, this, position: input.Position, consumedTokens: current.Position - input.Position);
        }
    }
}
