using System;
using System.Collections.Generic;
using System.Linq;

namespace CFGToolkit.ParserCombinator
{
    public static class Parser
    {
        public static IUnionResult<CharToken> TryParse<TValue>(this IParser<CharToken, TValue> parser, string input, GlobalState<CharToken> parState = null)
        {
            var tokens = new List<CharToken>();
            for (var i = 0; i < input.Length; i++)
            {
                tokens.Add(new CharToken() { StartIndex = i, EndIndex = i, Value = input[i] });
            }
            return TryParse(parser, tokens, parState);
        }

        public static IUnionResult<CharToken> TryParse<TValue>(this IParser<CharToken, TValue> parser, List<CharToken> tokens, GlobalState<CharToken> parState = null)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            GlobalState<CharToken> state = null;
            if (parState != null)
            {
                state = parState;
            }
            else
            {
                state = new GlobalState<CharToken>();
            }
            var inputObject = new Input(tokens);
            var parserState = new ParserState<CharToken>(new Frame<CharToken>() { Parser = parser, Input = inputObject }) { Name = "Root parser state" };
            var result = parser.Parse(inputObject, state, parserState);
            result.State = state;
            result.Input = inputObject;
            return result;
        }

        public static List<TValue> Parse<TValue>(this IParser<CharToken, TValue> parser, string input)
        {
            var tokens = new List<CharToken>();
            for (var i = 0; i < input.Length; i++)
            {
                tokens.Add(new CharToken() { StartIndex = i, EndIndex = i, Value = input[i] });
            }
            return Parse(parser, tokens);
        }

        public static List<TValue> Parse<TValue>(this IParser<CharToken, TValue> parser, List<CharToken> tokens)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            var result = TryParse(parser, tokens);

            if (result.WasSuccessful)
            {
                return result.Values.Select(v => v.GetValue<TValue>()).ToList();
            }

            throw new ParseException("Failed to parse");
        }
    }
}
