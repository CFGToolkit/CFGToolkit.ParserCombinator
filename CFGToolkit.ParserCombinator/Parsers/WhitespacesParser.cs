using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class WhitespacesParser : IParser<CharToken, List<char>>
    {
        public WhitespacesParser(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInput<CharToken> input, IGlobalState<CharToken> globalState, IParserState<CharToken> parserState)
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
