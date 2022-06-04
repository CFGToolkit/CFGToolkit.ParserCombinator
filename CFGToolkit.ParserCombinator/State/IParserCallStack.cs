using System.Collections.Generic;
using System.Threading;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public interface IParserCallStack<TToken> where TToken : IToken
    {
        Frame<TToken> Top { get; }

        string FullCallStackText { get; }

        List<Frame<TToken>> FullStack { get; }

        IParserCallStack<TToken> Parent { get; set; }

        bool IsPresent(string parserName, int depth);

        bool IsPresent(string[] parserName, int depth);

        Scope<TToken> CurrentScope { get; set; }

        IParserCallStack<TToken> Call(IParser<TToken> parser, IInputStream<TToken> input, CancellationTokenSource source = null, bool createLinkedTokenSource = false);

    }
}
