namespace CFGToolkit.ParserCombinator.Input
{
    public interface IToken
    {
        int StartIndex { get; set; }

        int EndIndex { get; set; }
    }

    public interface IToken<TValue> : IToken
    {
        public TValue Value { get; set; }
    }
}
