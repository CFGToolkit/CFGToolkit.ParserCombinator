using System;
using System.Text.RegularExpressions;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class RegexParser : BaseParser<CharToken, string>
    {
        private readonly Regex _regex;
        private readonly Func<string, bool> _predicate;
        private readonly bool _token;

        public RegexParser(string name, Regex regex, Func<string, bool> predicate, bool token = false)
        {
            Name = name;
            _regex = regex;
            _predicate = predicate;
            _token = token;
        }

        protected override IUnionResult<CharToken> ParseInternal(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            if (!input.AtEnd)
            {
                int prefixLen = 0;
                if (_token)
                {
                    prefixLen = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), input.Position);
                }
                Match match = _regex.Match(input.GetText(), input.Position + prefixLen);

                if (match.Success)
                {
                    if (_predicate(match.Value))
                    {
                        int suffixLen = 0;
                        if (_token)
                        {
                            suffixLen = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), input.Position + match.Length + prefixLen);
                        }

                        var remainder = input.Advance(match.Length + prefixLen + suffixLen);

                        return UnionResultFactory.Success(match.Value, remainder, this, input.Position, consumedTokens: match.Length + prefixLen + suffixLen);
                    }
                    else
                    {
                        return UnionResultFactory.Failure(this, "Failed to match predicate on regex", input);
                    }
                }

                return UnionResultFactory.Failure(this, "Failed to match regex", input);
            }

            return UnionResultFactory.Failure(this, "Failed to match regex. Unexpected end of input", input);
        }
    }
}
