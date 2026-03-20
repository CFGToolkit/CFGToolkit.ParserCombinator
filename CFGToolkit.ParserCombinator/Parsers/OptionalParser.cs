using System.Collections.Generic;
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
                bool hasEmptyMatch = false;

                var result = new List<IUnionResultValue<TToken>>(successValues.Count + 1);

                for (int i = 0; i < successValues.Count; i++)
                {
                    if (successValues[i].ConsumedTokens == 0)
                    {
                        hasEmptyMatch = true;
                    }
                }

                if (!Greedy && !hasEmptyMatch)
                {
                    result.Add(new UnionResultValue<TToken>(typeof(IOption<T>)) { Value = new None<T>(), Reminder = input, Position = input.Position, ConsumedTokens = 0, IsSuccessful = true });
                }

                for (int i = 0; i < successValues.Count; i++)
                {
                    var v = successValues[i];
                    result.Add(new UnionResultValue<TToken>(typeof(IOption<T>))
                    {
                        Value = new Some<T>((T)v.Value),
                        Position = v.Position,
                        ConsumedTokens = v.ConsumedTokens,
                        Reminder = v.Reminder,
                        IsSuccessful = true
                    });
                }

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
