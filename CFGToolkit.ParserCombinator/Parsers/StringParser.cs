using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class StringParser : IParser<CharToken, string>
    {
        private readonly string _string;
        private readonly bool _token;

        public StringParser(string name, string @string, bool token = false)
        {
            Name = name;
            _string = @string;
            _token = token;
        }

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInputStream<CharToken> input, IGlobalState<CharToken> globalState, IParserCallStack<CharToken> parserCallStack)
        {
            var current = input;
            int count = 0;

            if (_token)
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
            foreach (var @char in _string)
            {
                if (!current.AtEnd && current.Current.Value == @char)
                {
                    count++;
                    current = current.Advance();
                }
                else
                {
                    return UnionResultFactory.Failure(this, "Failed to match string: " + _string, input);
                }
            }

            if (_token)
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
            if (count == _string.Length)
            {
                return UnionResultFactory.Success(_string, current, this, position: input.Position, consumedTokens: current.Position - input.Position);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Failed to match string: " + _string, input);
            }
        }
    }
}
