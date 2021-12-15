using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.State
{
    public class Frame<TToken> where TToken : IToken
    {
        public Frame<TToken> Parent { get; set; }

        public IInputStream<TToken> Input { get; set; }

        public IParser<TToken> Parser { get; set; }

        public IUnionResult<TToken> Result { get; set; }

        public override string ToString()
        {
            return Parser.Name;
        }
    }
}
