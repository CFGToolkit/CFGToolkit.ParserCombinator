using System.Collections.Generic;
using System.Linq;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class EndParser<TToken, T> : IParser<TToken, T> where TToken : IToken
    {
        private readonly IParser<TToken, T> parser;

        public EndParser(string name, IParser<TToken, T> parser)
        {
            Name = name;
            this.parser = parser;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var result = parser.Parse(input, globalState, parserState.Call(parser, input));

            if (result.WasSuccessful)
            {
                var filteredValues = result.Values.Where(i => i.Reminder.AtEnd);

                if (filteredValues.Any())
                {
                    return UnionResultFactory.Success<TToken, T>(this, filteredValues.ToList());
                }
            }

            return UnionResultFactory.Failure(this, result.Values);
        }
    }
}
