namespace CFGToolkit.ParserCombinator
{
    using CFGToolkit.ParserCombinator.Parsers;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Parse
    {
        public static IParser<TToken, TResult> Sequence<TToken, TResult>(string name, Func<(string valueParserName, object value)[], TResult> select, params Func<IParser<TToken>>[] parserFactories) where TToken : IToken
        {
            return ParserFactory.CreateEventParser(new SequenceParser<TToken, TResult>(name, select, parserFactories));
        }

        public static IParser<TToken, IEnumerable<T>> DelimitedBy<TToken, T, U>(this IParser<TToken, T> parser, IParser<TToken, U> delimiter) where TToken : IToken
        {
            return DelimitedBy(parser, delimiter, null, null);
        }

        public static IParser<TToken, IEnumerable<T>> DelimitedBy<TToken, T, U>(this IParser<TToken, T> parser, IParser<TToken, U> delimiter, int? minimumCount, int? maximumCount) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return ObjectCache.CacheGet($"DelimitedBy({minimumCount},{maximumCount})", parser, delimiter, () =>
            {
                return ParserFactory.CreateEventParser(from head in parser.Once()
                                     from tail in
                                         (from separator in delimiter
                                          from item in parser
                                          select item).Repeat(minimumCount - 1, maximumCount - 1)
                                     select head.Concat(tail));
            });
        }

        public static IParser<TToken, List<T>> Repeat<TToken, T>(this IParser<TToken, T> parser, int count, bool greedy = true) where TToken : IToken
        {
            return Repeat(parser, count, count, greedy);
        }

        public static IParser<TToken, List<T>> Repeat<TToken, T>(this IParser<TToken, T> parser, int? minimumCount, int? maximumCount, bool greedy = true) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return ObjectCache.CacheGet($"Repeat({minimumCount},{maximumCount}, {greedy})", parser, () =>
            {
                string name = $"{ parser.Name } repeated " + (minimumCount.HasValue ? $" min = {minimumCount} " : " ") + (maximumCount.HasValue ? $" max = {maximumCount}" : "");

                return ParserFactory.CreateEventParser(new RepeatParser<TToken, T>(name, parser, minimumCount, maximumCount, greedy));
            });
        }
    }
}
