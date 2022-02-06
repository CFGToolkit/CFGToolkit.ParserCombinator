using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class ReturnParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        public ReturnParser(string name, TResult value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public TResult Value { get; }


        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            return UnionResultFactory.Success(Value, input, this, position: input.Position, consumedTokens: 0);
        }
    }

    public class ReturnParser<TToken, T, U> : IParser<TToken, U> where TToken : IToken
    {
        private readonly IParser<TToken, T> _parser;

        public ReturnParser(string name, IParser<TToken, T> parser, U value)
        {
            Name = name;
            _parser = parser;
            Value = value;
        }

        public string Name { get; set; }

        public U Value { get; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var result = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (result.WasSuccessful)
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
                return UnionResultFactory.Failure(this, "Parser failed", input);
            }
        }
    }
}
