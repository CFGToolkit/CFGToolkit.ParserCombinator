﻿using System;
using System.Text.RegularExpressions;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class RegexParser : IParser<CharToken, string>
    {
        private readonly Regex _regex;
        private readonly Func<string, bool> _predicate;

        public RegexParser(string name, Regex regex, Func<string, bool> predicate)
        {
            Name = name;
            _regex = regex;
            _predicate = predicate;
        }

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInput<CharToken> input, IGlobalState<CharToken> globalState, IParserState<CharToken> parserState)
        {
            if (!input.AtEnd)
            {
                var remainder = input;
                Match match = _regex.Match(((Input)input).SourceText, input.Position);

                if (match.Success)
                {
                    if (_predicate(match.Value))
                    {
                        remainder = remainder.Advance(match.Length);
                        return UnionResultFactory.Success(match.Value, remainder, this, input.Position, consumedTokens: match.Length);
                    }
                    else
                    {
                        return UnionResultFactory.Failure(this, $"Failed to match predicate on regex: {_regex}", input);
                    }
                }

                return UnionResultFactory.Failure(this, $"Failed to match: {_regex}", input);
            }

            return UnionResultFactory.Failure(this, "Failed to match: " + _regex + ". Unexpected end of input", input);
        }
    }
}
