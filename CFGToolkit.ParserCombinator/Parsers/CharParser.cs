using System;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class CharParser : IParser<CharToken, char>
    {
        private readonly Predicate<char> _predicate;
        private readonly string _description;
        private readonly bool _token;

        public CharParser(string name, Predicate<char> predicate, string description, bool token = false)
        {
            Name = name;
            _predicate = predicate;
            _description = description;
            _token = token;
        }

        public string Name { get; set; }

        public IUnionResult<CharToken> Parse(IInput<CharToken> input, IGlobalState<CharToken> globalState, IParserState<CharToken> parserState)
        {
            var current = input;

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

            char c = '0';
            if (!current.AtEnd)
            {
                c = current.Current.Value;
                if (!_predicate(c))
                {
                    return UnionResultFactory.Failure(this, Name + " failed.", input);
                }
                current = current.Advance();
            }
            else
            {
                return UnionResultFactory.Failure(this, Name + " failed. End of input unexpected.", input);
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

            return UnionResultFactory.Success(c, current, this, position: input.Position, consumedTokens: current.Position - input.Position);
        }
    }
}
