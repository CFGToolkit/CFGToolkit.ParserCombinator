using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class WhitespacesParser : IParser<CharToken, List<char>>
    {
        public WhitespacesParser(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public IUnionResult<CharToken> Parse(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            var current = input.AdvanceWhile(token => char.IsWhiteSpace(token.Value), true, out var length);

            return UnionResultFactory.Success(null, current, this, input.Position, consumedTokens: current.Position - input.Position);
        }
    }
}
