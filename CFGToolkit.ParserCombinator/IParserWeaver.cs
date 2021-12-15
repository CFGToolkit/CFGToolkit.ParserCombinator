using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator
{
    public interface IParserWeaver
    {
        IParser<TToken, TResult> Create<TToken, TResult>(IParser<TToken, TResult> parser) where TToken : IToken;
    }
}
