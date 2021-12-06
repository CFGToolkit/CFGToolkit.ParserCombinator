using System.Linq;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class StringParser : IParser<CharToken, string>
    {
        private readonly string @string;
        private readonly bool token;

        public StringParser(string name, string @string, bool token = false)
        {
            Name = name;
            this.@string = @string;
            this.token = token;
        }

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInput<CharToken> input, IGlobalState<CharToken> globalState, IParserState<CharToken> parserState)
        {
            var current = input;
            int count = 0;

            if (token)
            {
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
            }
            foreach (var @char in @string)
            {
                if (!current.AtEnd && current.Current.Value == @char)
                {
                    count++;
                    current = current.Advance();
                }
                else
                {
                    return UnionResultFactory.Failure(this, "Failed to match string: " + @string, input);
                }
            }

            if (token)
            {
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
            }
            if (count == @string.Length)
            {
                return UnionResultFactory.Success(@string, current, this, position: input.Position, consumedTokens: current.Position - input.Position);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Failed to match string: " + @string, input);
            }
        }
    }
}
