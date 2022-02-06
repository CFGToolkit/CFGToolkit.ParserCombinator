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
                tokens.Add(new CharToken() { Position = i, Value = input[i] });
            }
            return TryParse(parser, tokens, parState);
        }

        public static IUnionResult<TToken> TryParse<TToken, TValue>(this IParser<TToken, TValue> parser, List<TToken> tokens, GlobalState<TToken> parState = null) where TToken: IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            GlobalState<TToken> state = null;
            if (parState != null)
            {
                state = parState;
            }
            else
            {
                state = new GlobalState<TToken>();
            }

            if (Options.Cache && state.Cache == null)
            {
                state.Cache = new Dictionary<long, IUnionResult<TToken>>[tokens.LongCount()];
                for (var i = 0; i < tokens.LongCount(); i++)
                {
                    state.Cache[i] = new Dictionary<long, IUnionResult<TToken>>();
                }
            }

            var inputStream = new InputStream<TToken>(tokens, 0, new Dictionary<string, object>());

            var parserCallStack = new ParserCallStack<TToken>(new Frame<TToken>() { Parser = parser, Input = inputStream });
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
                tokens.Add(new CharToken() { Position = i, Value = input[i] });
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
