using System.Threading;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public class Frame<TToken> where TToken : IToken
    {
        public Frame<TToken> Parent { get; set; }

        public bool ShouldContinue { get; set; } = true;

        public IInputStream<TToken> Input { get; set; }

        public IParser<TToken> Parser { get; set; }

        public IUnionResult<TToken> Result { get; set; }

        public CancellationTokenSource TokenSource { get; set; }

        public override string ToString()
        {
            return Parser.Name;
        }
    }
}
