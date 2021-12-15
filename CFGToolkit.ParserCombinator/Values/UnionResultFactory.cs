using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.Values
{
    public static class UnionResultFactory
    {
        public static UnionResult<TToken> Success<TToken, TResult>(TResult value, IInputStream<TToken> remainder, IParser<TToken, TResult> parser, int position, int consumedTokens) where TToken : IToken
        {
            return new UnionResult<TToken>(typeof(TResult))
            {
                Parser = parser,
                Values = new List<IUnionResultValue<TToken>>
                {
                    new UnionResultValue<TToken>(typeof(TResult))
                    {
                        Value = value,
                        Position = position,
                        Reminder = remainder,
                        ConsumedTokens = consumedTokens,
                    }
                },
            };
        }

        public static UnionResult<CharToken> Success<TResult>(TResult value, IInputStream<CharToken> remainder, IParser<CharToken, TResult> parser, int position, int consumedTokens)
        {
            return Success<CharToken, TResult>(value, remainder, parser, position, consumedTokens);
        }

        public static UnionResult<TToken> Success<TToken, TResult>(IParser<TToken, TResult> parser, List<IUnionResultValue<TToken>> values) where TToken : IToken
        {
            return new UnionResult<TToken>(typeof(TResult))
            {
                Parser = parser,
                Values = values,
            };
        }

        public static UnionResult<TToken> Success<TToken, TResult>(IParser<TToken, TResult> parser, IUnionResult<TToken> value) where TToken : IToken
        {
            return new UnionResult<TToken>(typeof(TResult))
            {
                Parser = parser,
                Values = value.Values,
            };
        }

        public static UnionResult<TToken> Failure<TToken, TResult>(IParser<TToken, TResult> parser, string errorMessage, IInputStream<TToken> input) where TToken : IToken
        {
            return new UnionResult<TToken>(typeof(TResult))
            {
                Parser = parser,
                ErrorMessage = errorMessage,
                Input = input,
                Values = new List<IUnionResultValue<TToken>>()
            };
        }
    }
}
