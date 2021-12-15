using System;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator
{
    public static partial class Parser
    {
        public static IParser<TToken, IOption<T>> Optional<TToken, T>(this IParser<TToken, T> parser, bool greedy = false) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return Weaver.Create(new OptionalParser<TToken, T>(parser, "Optional: (" + parser.Name + ")", greedy));
        }
    }
}
