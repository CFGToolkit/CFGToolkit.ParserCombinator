using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public interface IParserCallStack<TToken> where TToken : IToken
    {
        Frame<TToken> Top { get; }

        string FullCallStackText { get; }

        Stack<Frame<TToken>> FullStack { get; }

        IParserCallStack<TToken> Parent { get; set; }

        bool HasParser(string parserName, int depth);

        IParserCallStack<TToken> Call(IParser<TToken> parser, IInputStream<TToken> input);
    }
}
