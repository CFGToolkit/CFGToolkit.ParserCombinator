using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

namespace CFGToolkit.ParserCombinator
{
    public interface IParser<TToken> where TToken : IToken
    {
        string Name { get; set; }

        IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack);
    }

    public interface IParser<TToken, TResult> : IParser<TToken> where TToken : IToken
    {
    }
}
