using CFGToolkit.ParserCombinator.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CFGToolkit.ParserCombinator
{
    public static partial class Parse
    {
        public static IParser<TToken, TResult> Except<TToken, T, TResult>(this IParser<TToken, TResult> parser, IParser<TToken, T> except) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (except == null) throw new ArgumentNullException(nameof(except));

            string name = "Except: " + parser.Name + ", " + except;
            return ParserFactory.CreateEventParser(new ExceptParser<TToken, T, TResult>(name, parser, except));
        }

        public static IParser<CharToken, char> Char(Predicate<char> predicate, string description, bool token = false)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            string name = "Char: " + description;
            return ParserFactory.CreateEventParser(new CharParser(name, predicate, description, token));
        }

        public static IParser<CharToken, char> CharExcept(Predicate<char> predicate, string description)
        {
            return Char(c => !predicate(c), description);
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
            return CharExcept(ch => c == ch, "Except : "+ c);
        }

        public static IParser<CharToken, char> CharExcept(string c)
        {
            return CharExcept(c.Contains, "Except: " + c);
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
            return ParserFactory.CreateEventParser(new StringParser(name, @string, token));
        }

        public static IParser<CharToken, List<char>> WhiteSpaces()
        {
            return ParserFactory.CreateEventParser(new WhitespacesParser("Whitespaces"));
        }

        public static IParser<TToken, object> Not<TToken, T>(this IParser<TToken, T> parser) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            string name = "Not: " + parser.Name;
            return ParserFactory.CreateEventParser(new NotParser<TToken>(name, parser));
        }

        public static IParser<TToken, U> Then<TToken, T, U>(this IParser<TToken, T> first, Func<T, IParser<TToken, U>> second) where TToken : IToken
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return ParserFactory.CreateEventParser(new ThenFuncPParser<TToken, T, U>("Then: " + first + " => func parser", first, second));
        }

        public static IParser<TToken, U> Then<TToken, T, U>(this IParser<TToken, T> first, Func<T, U> second) where TToken : IToken
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return ParserFactory.CreateEventParser(new ThenFuncParser<TToken, T, U>("Then: " + first.Name + " => func", first, second));
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

            return ParserFactory.CreateEventParser(parser.Then(t => convert(t)).Named("Select: " + parser.Name));
        }

        public static IParser<TToken, TBase> Cast<TToken, TBase, TDerive>(this IParser<TToken, TDerive> parser) where TDerive : TBase where TBase : class where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return ParserFactory.CreateEventParser(new CastParser<TToken, TBase, TDerive>("Cast: " + typeof(TDerive).Name + "=>" + typeof(TBase).Name, parser));
        }

        public static IParser<CharToken, T> TokenWithoutNewLines<T>(this IParser<CharToken, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return ParserFactory.CreateEventParser(
                (from leading in Char(c => c != '\r' && c != '\n' && char.IsWhiteSpace(c), "Whitespace without new lines").Many()
                 from item in parser
                 from trailing in Char(c => c != '\r' && c != '\n' && char.IsWhiteSpace(c), "Whitespace without new lines").Many()
                 select item).Named($"TokenWithoutNewLines: ({parser})"));
        }

        public static IParser<CharToken, T> Token<T>(this IParser<CharToken, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return Sequence("Token: " + parser.Name, (args) => (T)args[1].value, new Lazy<IParser<CharToken>>(() => WhiteSpaces()), new Lazy<IParser<CharToken>>(() => parser), new Lazy<IParser<CharToken>>(() => WhiteSpaces()));
        }

        public static IParser<CharToken, string> Text(this IParser<CharToken, List<char>> characters)
        {
            return ParserFactory.CreateEventParser(characters.Then(chs => new string(chs.ToArray())).Named("Text"));
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
            return ParserFactory.CreateEventParser(new OrParser<TToken, TResult>(name, first, second));
        }

        public static IParser<TToken, T> XOr<TToken, T>(this IParser<TToken, T> first, IParser<TToken, T> second) where TToken : IToken
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            string name = first.Name + " xor " + second.Name;
            return ParserFactory.CreateEventParser(new XOrParser<TToken, T>(name, first, second));
        }

        public static IParser<TToken, TResult> Named<TToken, TResult>(this IParser<TToken, TResult> parser, string name) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (name == null) throw new ArgumentNullException(nameof(name));

            parser.Name = name;

            return parser;
        }

        public static IParser<TToken, List<T>> Once<TToken, T>(this IParser<TToken, T> parser) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            string name = "Once: (" + parser.Name + ")";
            return ParserFactory.CreateEventParser(new OnceParser<TToken, T>(name, parser));
        }

        public static IParser<TToken, T> Return<TToken, T>(T value) where TToken : IToken
        {
            string name = "Return: " + value;
            return ParserFactory.CreateEventParser(new ReturnParser<TToken, T>(name, value));
        }

        public static IParser<CharToken, T> Return<T>(T value) 
        {
            return Return<CharToken, T>(value);
        }

        public static IParser<TToken, U> Return<TToken, T, U>(this IParser<TToken, T> parser, U value) where TToken : IToken
        {
            string name = "Return: " + value;
            return ParserFactory.CreateEventParser(new ReturnParser<TToken, T, U>(name, parser, value));
        }

        public static IParser<TToken, T> Where<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            string name = parser.Name + " where " + predicate;
            return ParserFactory.CreateEventParser(new WhereParser<TToken, T>(name, parser, predicate));
        }

        public static IParser<TToken, V> SelectMany<TToken, T, U, V>(
            this IParser<TToken, T> parser,
            Func<T, IParser<TToken, U>> selector,
            Func<T, U, V> projector) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return ParserFactory.CreateEventParser(new SelectManyParser<TToken, T, U, V>(parser, selector, projector));
        }

        public static IParser<TToken, T> End<TToken, T>(this IParser<TToken, T> parser) where TToken : IToken
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
         
            return ParserFactory.CreateEventParser(new EndParser<TToken, T>("End: " + parser.Name, parser));
        }
    }
}
