using System;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator
{
    public static partial class Parser
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
            var inputStream = new InputStream(tokens);
            var parserCallStack = new ParserCallStack<CharToken>(new Frame<CharToken>() { Parser = parser, Input = inputStream });
            var result = parser.Parse(inputStream, state, parserCallStack);
            result.GlobalState = state;
            result.Input = inputStream;
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

            throw new ParserException("Failed to parse. Last consumed position: " + result.GlobalState.LastConsumedPosition);
        }
    }
}
