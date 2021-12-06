using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator
{
    public interface IParserState<TToken> where TToken : IToken
    {
        string Name { get; set; }

        Frame<TToken> Frame { get; }

        string FullCallStackText { get; }

        Stack<Frame<TToken>> FullCallStack { get; }

        ParserState<TToken> Parent { get; set; }

        bool HasParser(string parserName, int depth = int.MaxValue);

        ParserState<TToken> Call(IParser<TToken> parser, IInput<TToken> input);
    }
}