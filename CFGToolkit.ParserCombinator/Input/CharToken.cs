namespace CFGToolkit.ParserCombinator.Input
{
    public class CharToken : IToken<char>
    {
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public char Value { get; set; }

        public string Text { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
