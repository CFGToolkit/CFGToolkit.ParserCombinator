namespace CFGToolkit.ParserCombinator
{
    public class CharToken : IToken<char>
    {
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public char Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
