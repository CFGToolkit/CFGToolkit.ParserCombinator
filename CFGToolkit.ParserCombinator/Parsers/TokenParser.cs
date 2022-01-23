using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class TokenParser<TResult> : IParser<CharToken, TResult>
    {
        private readonly IParser<CharToken, TResult> _parser;

        public TokenParser(string name, IParser<CharToken, TResult> parser)
        {
            Name = name;
            _parser = parser;
        }

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            int start = input.Position;
            var current = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), true, out var prefix);
            var result = _parser.Parse(current, globalState, parserCallStack.Call(_parser, current));

            if (result.WasSuccessful)
            {
                foreach (var value in result.Values)
                {
                    value.Reminder.AdvanceWhile(token => char.IsWhiteSpace(token.Value), false, out var whitespaces);
                    value.Position = start;
                    value.ConsumedTokens += prefix + whitespaces;
                }

                return UnionResultFactory.Success(this, result);
            }
            return UnionResultFactory.Failure(this, $"Fail to match {Name} token", input);
        }
    }
}
