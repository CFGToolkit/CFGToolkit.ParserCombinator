using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers;

namespace CFGToolkit.ParserCombinator
{
    public static partial class Parser
    {
        public static IParser<TToken, TResult> Except<TToken, T, TResult>(this IParser<TToken, TResult> parser, IParser<TToken, T> except) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (except == null) throw new ArgumentNullException(nameof(except));

            string name = "Except: " + parser.Name + ", " + except;
            return new ExceptParser<TToken, T, TResult>(name, parser, except);
        }

        public static IParser<CharToken, char> Char(Predicate<char> predicate, string description, bool token = false)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            string name = "Char: " + description;
            return new CharParser(name, predicate, description, token) { ShouldUpdateGlobalState = Options.CharLevelReporting };
        }

        public static IParser<CharToken, char> Char(char c, bool token = false)
        {
            return Char(ch => c == ch, c.ToString(), token);
        }

        public static IParser<CharToken, char> Chars(string c)
        {
            return Char(c.Contains, "String: " + c);
        }

        public static IParser<CharToken, char> CharExcept(char c)
        {
            return CharExcept(ch => c == ch, "Except : " + c);
        }

        public static IParser<CharToken, char> CharExcept(string c)
        {
            return CharExcept(c.Contains, "Except: " + c);
        }

        public static IParser<CharToken, char> CharExcept(Predicate<char> predicate, string description)
        {
            return Char(c => !predicate(c), description);
        }

        public static IParser<CharToken, char> AnyChar()
        {
            return Char(c => true, "Any char");
        }

        public static readonly IParser<CharToken, char> WhiteSpace = Char(char.IsWhiteSpace, "Whitespace");

        public static readonly IParser<CharToken, char> Digit = Char(char.IsDigit, "Digit");

        public static readonly IParser<CharToken, char> Letter = Char(char.IsLetter, "Letter");

        public static readonly IParser<CharToken, char> LetterOrDigit = Char(char.IsLetterOrDigit, "Letter or digit");

        public static readonly IParser<CharToken, char> Lower = Char(char.IsLower, "Lower letter");

        public static readonly IParser<CharToken, char> Upper = Char(char.IsUpper, "Upper letter");

        public static readonly IParser<CharToken, char> Numeric = Char(char.IsNumber, "Numeric");

        public static IParser<CharToken, string> String(string @string, bool token = false)
        {
            if (@string == null) throw new ArgumentNullException(nameof(@string));

            string name = "String: " + @string;
            return new StringParser(name, @string, token);
        }

        public static IParser<CharToken, List<char>> WhiteSpaces = new WhitespacesParser("Whitespaces");

        public static IParser<TToken, object> Not<TToken, T>(this IParser<TToken, T> parser) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            string name = "Not: " + parser.Name;
            return new NotParser<TToken>(name, parser);
        }

        public static IParser<TToken, U> Then<TToken, T, U>(this IParser<TToken, T> first, Func<T, IParser<TToken, U>> second) where TToken : IToken
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return new ThenFuncPParser<TToken, T, U>("Then: " + first + " => func parser", first, second);
        }

        public static IParser<TToken, U> Then<TToken, T, U>(this IParser<TToken, T> first, Func<T, U> second) where TToken : IToken
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return new ThenFuncParser<TToken, T, U>("Then: " + first.Name + " => func", first, second);
        }

        public static IParser<TToken, List<T>> Many<TToken, T>(this IParser<TToken, T> parser, bool greedy = true) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Repeat(0, null, greedy).Named("Many: (" + parser.Name + ")");
        }

        public static IParser<TToken, U> Select<TToken, T, U>(this IParser<TToken, T> parser, Func<T, U> convert) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (convert == null) throw new ArgumentNullException(nameof(convert));

            return parser.Then(t => convert(t)).Named("Select: " + parser.Name);
        }

        public static IParser<TToken, TBase> Cast<TToken, TBase, TDerive>(this IParser<TToken, TDerive> parser) where TDerive : TBase where TBase : class where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return new CastParser<TToken, TBase, TDerive>("Cast: " + typeof(TDerive).Name + "=>" + typeof(TBase).Name, parser);
        }

        public static IParser<CharToken, T> TokenWithoutNewLines<T>(this IParser<CharToken, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return 
                (from leading in Char(c => c != '\r' && c != '\n' && char.IsWhiteSpace(c), "Whitespace without new lines").Many()
                 from item in parser
                 from trailing in Char(c => c != '\r' && c != '\n' && char.IsWhiteSpace(c), "Whitespace without new lines").Many()
                 select item).Named($"TokenWithoutNewLines: ({parser})");
        }

        public static IParser<CharToken, T> Token<T>(this IParser<CharToken, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return new TokenParser<T>("Token: " + parser.Name, parser);
        }

        public static IParser<TToken, T> Cached<TToken, T>(this IParser<TToken, T> parser, long id) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            if (Options.Cache)
            {
                return new CachedParser<TToken, T>("Cached: " + parser.Name, parser, id);
            }
            return parser;
        }

        public static IParser<CharToken, string> Text(this IParser<CharToken, List<char>> characters)
        {
            return characters.Then(chs => new string(chs.ToArray())).Named("Text");
        }

        public static IParser<CharToken, string> Text(this IParser<CharToken, string> parser)
        {
            return parser;
        }

        public static IParser<TToken, TResult> Or<TToken, TResult>(this IParser<TToken, TResult> first, IParser<TToken, TResult> second) where TToken : IToken
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            string name = "(" + first.Name + ") or (" + second.Name + ")";
            return new OrParser<TToken, TResult>(name, first, second);
        }

        public static IParser<TToken, T> Or<TToken, T>(string name, bool parallel, params IParser<TToken, T>[] parsers) where TToken : IToken
        {
            if (Options.MultiThreading && parallel)
            {
                return new OrMultipleParallelParser<TToken, T>(name, parsers);
            }
            else
            {
                return new OrMultipleParser<TToken, T>(name, parsers);
            }
        }

        public static IParser<TToken, T> XOr<TToken, T>(this IParser<TToken, T> first, IParser<TToken, T> second) where TToken : IToken
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            string name = first.Name + " xor " + second.Name;
            return new XOrParser<TToken, T>(name, first, second);
        }

        public static IParser<TToken, T> XOr<TToken, T>(string name, XOrParallelMode mode, params IParser<TToken, T>[] parsers) where TToken : IToken
        {
            if (Options.MultiThreading && (mode == XOrParallelMode.Ordered || mode == XOrParallelMode.First))
            {
                if (mode == XOrParallelMode.First)
                {
                    return new XOrMultipleFirstParallelParser<TToken, T>(name, parsers);
                }
                else
                {
                    return new XOrMultipleParallelParser<TToken, T>(name, parsers);
                }
            }
            else
            {
                return new XOrMultipleParser<TToken, T>(name, parsers);
            }
        }

        public static IParser<TToken, TResult> Named<TToken, TResult>(this IParser<TToken, TResult> parser, string name) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (name == null) throw new ArgumentNullException(nameof(name));

            parser.Name = name;

            return parser;
        }

        public static IParser<TToken, TResult> Tag<TToken, TResult>(this IParser<TToken, TResult> parser, string tagName, string tagValue = null) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (tagName == null) throw new ArgumentNullException(nameof(tagName));

            if (parser.Tags == null)
            {
                parser.Tags = new Dictionary<string, string>();
            }

            parser.Tags[tagName] = tagValue;

            return parser;
        }

        public static IParser<TToken, List<T>> Once<TToken, T>(this IParser<TToken, T> parser) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            string name = "Once: (" + parser.Name + ")";
            return new OnceParser<TToken, T>(name, parser);
        }

        public static IParser<TToken, T> Return<TToken, T>(T value) where TToken : IToken
        {
            string name = "Return: " + value;
            return new ReturnParser<TToken, T>(name, value);
        }

        public static IParser<CharToken, T> Return<T>(T value)
        {
            return Return<CharToken, T>(value);
        }

        public static IParser<TToken, U> Return<TToken, T, U>(this IParser<TToken, T> parser, U value) where TToken : IToken
        {
            string name = "Return: " + value;
            return new ReturnParser<TToken, T, U>(name, parser, value);
        }

        public static IParser<TToken, T> Where<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            string name = parser.Name + " where " + predicate;
            return new WhereParser<TToken, T>(name, parser, predicate);
        }

        public static IParser<TToken, V> SelectMany<TToken, T, U, V>(
            this IParser<TToken, T> parser,
            Func<T, IParser<TToken, U>> selector,
            Func<T, U, V> projector) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return new SelectManyParser<TToken, T, U, V>(parser, selector, projector);
        }

        public static IParser<TToken, T> End<TToken, T>(this IParser<TToken, T> parser) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return new EndParser<TToken, T>("End: " + parser.Name, parser);
        }
    }
}
