using System;
using CFGToolkit.ParserCombinator.Parsers;

namespace CFGToolkit.ParserCombinator
{
    partial class Parse
    {
        public static IParser<TToken, IOption<T>> Optional<TToken, T>(this IParser<TToken, T> parser, bool greedy = false) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return ParserFactory.CreateEventParser(new OptionalParser<TToken, T>(parser, "Optional: (" + parser.Name + ")", greedy));
        }
    }
}
