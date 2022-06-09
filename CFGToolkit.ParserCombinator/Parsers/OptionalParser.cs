using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class OptionalParser<TToken, T> : BaseParser<TToken, IOption<T>> where TToken : IToken
    {
        private readonly IParser<TToken, T> _current;

        public OptionalParser(IParser<TToken, T> current, string name, bool greedy = false)
        {
            _current = current;
            Name = name;
            Greedy = greedy;
        }

        public bool Greedy { get; }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var results = _current.Parse(input, globalState, parserCallStack.Call(_current, input));

            if (results.IsSuccessful)
            {
                var successValues = results.Values;
                var optionValues = successValues
                    .Select(v => (IUnionResultValue<TToken>)new UnionResultValue<TToken>(typeof(IOption<T>))
                    {
                        Value = new Some<T>((T)v.Value),
                        Position = v.Position,
                        ConsumedTokens = v.ConsumedTokens,
                        Reminder = v.Reminder,
                        IsSuccessful = true
                    });

                var result = new List<IUnionResultValue<TToken>>(optionValues.Count());

                if (!Greedy && !successValues.Any(item => item.ConsumedTokens == 0))
                {
                    result.Add(new UnionResultValue<TToken>(typeof(IOption<T>)) { Value = new None<T>(), Reminder = input, Position = input.Position, ConsumedTokens = 0, IsSuccessful = true });
                }
                result.AddRange(optionValues);

                return UnionResultFactory.Success(this, result);
            }
            else
            {
                var result = new List<IUnionResultValue<TToken>>
                {
                    new UnionResultValue<TToken>(typeof(IOption<T>))
                    {
                        ConsumedTokens = 0,
                        Value = new None<T>(),
                        Reminder = input,
                        Position = input.Position,
                        IsSuccessful = true
                    }
                };

                return UnionResultFactory.Success(this, result);
            }
        }
    }
}
