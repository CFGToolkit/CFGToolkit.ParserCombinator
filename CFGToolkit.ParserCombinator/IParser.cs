namespace CFGToolkit.ParserCombinator
{
    public interface IParser<TToken> where TToken : IToken
    {
        string Name { get; set; }

        IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState);
    }

    public interface IParser<TToken, TResult> : IParser<TToken> where TToken : IToken
    {
    }
}
