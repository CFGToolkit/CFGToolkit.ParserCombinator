namespace CFGToolkit.ParserCombinator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CFGToolkit.ParserCombinator.Input;
    using CFGToolkit.ParserCombinator.Parsers;
    using System.Threading;

    public partial class Parser
    {
        public static IParser<TToken, TResult> Sequence<TToken, TResult>(string name, Func<(string valueParserName, object value)[], TResult> select, params Lazy<IParser<TToken>>[] parserFactories) where TToken : IToken
        {
            return new SequenceParser<TToken, TResult>(name, select, parserFactories);
        }

        public static IParser<TToken, IEnumerable<T>> DelimitedBy<TToken, T, U>(this IParser<TToken, T> parser, IParser<TToken, U> delimiter) where TToken : IToken
        {
            return DelimitedBy(parser, delimiter, null, null);
        }

        public static IParser<TToken, IEnumerable<T>> DelimitedBy<TToken, T, U>(this IParser<TToken, T> parser, IParser<TToken, U> delimiter, int? minimumCount, int? maximumCount) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));


            return from head in parser.Once()
                                                   from tail in
                                                       (from separator in delimiter
                                                        from item in parser
                                                        select item).Repeat(minimumCount - 1, maximumCount - 1)
                                                   select head.Concat(tail);
        }

        public static IParser<TToken, List<T>> Repeat<TToken, T>(this IParser<TToken, T> parser, int count, bool greedy = true) where TToken : IToken
        {
            return Repeat(parser, count, count, greedy);
        }

        public static IParser<TToken, List<T>> Repeat<TToken, T>(this IParser<TToken, T> parser, int? minimumCount, int? maximumCount, bool greedy = true) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            string name = $"{ parser.Name } repeated " + (minimumCount.HasValue ? $" min = {minimumCount} " : " ") + (maximumCount.HasValue ? $" max = {maximumCount}" : "");

            return new RepeatParser<TToken, T>(name, parser, minimumCount, maximumCount, greedy);
        }
    }
}
