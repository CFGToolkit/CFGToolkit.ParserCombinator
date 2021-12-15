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

        public IUnionResult<CharToken> Parse(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            var current = input;
            for (; ; )
            {
                if (current.AtEnd)
                {
                    break;
                }
                else
                {
                    if (char.IsWhiteSpace(current.Current.Value))
                    {
                        current = current.Advance();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return UnionResultFactory.Success(null, current, this, input.Position, consumedTokens: current.Position - input.Position);
        }
    }
}
