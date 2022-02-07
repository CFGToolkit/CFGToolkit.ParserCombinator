using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator
{
    public interface IParserWeaver
    {
        IParser<TToken, TResult> Create<TToken, TResult>(IParser<TToken, TResult> parser, bool updateState = true) where TToken : IToken;
    }
}
