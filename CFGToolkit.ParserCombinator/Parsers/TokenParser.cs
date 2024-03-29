﻿using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class TokenParser<TResult> : BaseParser<CharToken, TResult>
    {
        private readonly IParser<CharToken, TResult> _parser;

        public TokenParser(string name, IParser<CharToken, TResult> parser)
        {
            Name = name;
            _parser = parser;
        }

        protected override IUnionResult<CharToken> ParseInternal(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            int start = input.Position;
            input = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), true, out var prefix);

            var result = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (result.IsSuccessful)
            {
                foreach (var value in result.Values)
                {
                    value.Position = start;
                    value.Reminder = value.Reminder.AdvanceWhile(token => char.IsWhiteSpace(token.Value), true, out var suffix);
                    value.ConsumedTokens += suffix + prefix;
                }

                return UnionResultFactory.Success(this, result);
            }
            return UnionResultFactory.Failure(this, "Failed to parse", result.MaxConsumed, input.Position);
        }
    }
}
