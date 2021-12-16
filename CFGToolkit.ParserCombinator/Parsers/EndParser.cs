using System.Linq;
using CFGToolkit.ParserCombinator;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class EndParser<TToken, T> : IParser<TToken, T> where TToken : IToken
    {
        private readonly IParser<TToken, T> _parser;

        public EndParser(string name, IParser<TToken, T> parser)
        {
            Name = name;
            _parser = parser;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var result = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (result.WasSuccessful)
            {
                var filteredValues = result.Values.Where(i => i.Reminder.AtEnd);

                if (filteredValues.Any())
                {
                    return UnionResultFactory.Success<TToken, T>(this, filteredValues.ToList());
                }
                else
                {
                    return UnionResultFactory.Failure(this, $"Parser {_parser.Name} doesn't parse all input in {Name} parser", input);
                }
            }
            else
            {
                return UnionResultFactory.Failure(this, $"Parser {_parser.Name} failed in {Name} parser", input);
            }
        }
    }
}
