namespace CFGToolkit.ParserCombinator
{
    public interface IToken
    {
        int StartIndex { get; set; }

        int EndIndex { get; set; }

        string ToString();
    }

    public interface IToken<TValue> : IToken
    {
        public TValue Value { get; set; }
    }
}
