using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class EndParser<TToken, T> : BaseParser<TToken, T> where TToken : IToken
    {
        private readonly IParser<TToken, T> _parser;

        public EndParser(string name, IParser<TToken, T> parser)
        {
            Name = name;
            _parser = parser;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var result = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (result.IsSuccessful)
            {
                var filteredValues = new List<IUnionResultValue<TToken>>();
                foreach (var item in result.Values)
                {
                    if (item.Reminder.AtEnd)
                    {
                        filteredValues.Add(item);
                    }
                }

                if (filteredValues.Count > 0)
                {
                    return UnionResultFactory.Success<TToken, T>(this, filteredValues);
                }

                return UnionResultFactory.Failure(this, "Parser doesn't parse all input", result.MaxConsumed, input.Position);
            }

            return UnionResultFactory.Failure(this, "Parser failed", result.MaxConsumed, input.Position);
        }
    }
}
