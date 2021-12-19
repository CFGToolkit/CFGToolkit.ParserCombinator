using System;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class TokenParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly Predicate<TToken> _predicate;

        public TokenParser(string name, Predicate<TToken> predicate, Func<TToken, TResult> factory)
        {
            Name = name;
            _predicate = predicate;
            Factory = factory;
        }

        public string Name { get; set; }
        public Func<TToken, TResult> Factory { get; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var tmp = input;
            if (!tmp.AtEnd)
            {
                if (!_predicate(tmp.Current))
                {
                    return UnionResultFactory.Failure(this, Name + " failed.", input);
                }
                var t = tmp.Current;
                tmp = tmp.Advance();

                return UnionResultFactory.Success(Factory(t), tmp, this, position: input.Position, consumedTokens: 1);
            }
            else
            {
                return UnionResultFactory.Failure(this, Name + " failed. End of input unexpected.", input);
            }
        }
    }
}
