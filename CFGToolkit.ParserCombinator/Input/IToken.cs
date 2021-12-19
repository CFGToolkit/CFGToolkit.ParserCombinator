namespace CFGToolkit.ParserCombinator.Input
{
    public interface IToken
    {
        int Position { get; set; }
    }

    public interface IToken<TValue> : IToken
    {
        public TValue Value { get; set; }
    }
}
