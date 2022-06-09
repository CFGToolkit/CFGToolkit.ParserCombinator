using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class ReturnParser<TToken, TResult> : BaseParser<TToken, TResult> where TToken : IToken
    {
        public ReturnParser(string name, TResult value)
        {
            Name = name;
            Value = value;
        }

        public TResult Value { get; }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            return UnionResultFactory.Success(Value, input, this, position: input.Position, consumedTokens: 0);
        }
    }

    public class ReturnParser<TToken, T, U> : BaseParser<TToken, U> where TToken : IToken
    {
        private readonly IParser<TToken, T> _parser;

        public ReturnParser(string name, IParser<TToken, T> parser, U value)
        {
            Name = name;
            _parser = parser;
            Value = value;
        }

        public U Value { get; }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var result = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (result.IsSuccessful)
            {
                var values = new List<IUnionResultValue<TToken>>();

                foreach (var item in result.Values)
                {
                    values.Add(new UnionResultValue<TToken>(typeof(U))
                    {
                        Value = Value,
                        ConsumedTokens = item.ConsumedTokens,
                        Reminder = item.Reminder,
                        Position = item.Position,
                    });
                }

                return UnionResultFactory.Success<TToken, U>(this, values);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Parser failed", result.MaxConsumed, input.Position);
            }
        }
    }
}
