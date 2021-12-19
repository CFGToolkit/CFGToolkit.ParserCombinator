namespace CFGToolkit.ParserCombinator.Input
{
    public class CharToken : IToken<char>
    {
        public char Value { get; set; }

        public int Position { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
