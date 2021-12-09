namespace CFGToolkit.ParserCombinator
{
    public class Frame<TToken> where TToken : IToken
    {
        public Frame<TToken> Parent { get; set; }

        public IInput<TToken> Input { get; set; }

        public IParser<TToken> Parser { get; set; }

        public override string ToString()
        {
            return Parser.Name;
        }
    }
}
