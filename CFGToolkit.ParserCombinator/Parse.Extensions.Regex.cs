﻿using CFGToolkit.ParserCombinator.Parsers;
using System;
using System.Text.RegularExpressions;

namespace CFGToolkit.ParserCombinator
{
    partial class Parse
    {
        public static IParser<CharToken, string> Regex(string pattern, RegexOptions options = RegexOptions.None)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return RegexMatch("Pattern: " + pattern, new Regex(pattern, options));
        }

        public static IParser<CharToken, string> RegexMatch(string pattern, RegexOptions options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return RegexMatch(pattern, new Regex(pattern, options));
        }

        public static IParser<CharToken, string> RegexMatch(string name, Regex regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));
            regex = OptimizeRegex(regex);
            return ParserFactory.CreateEventParser(new RegexParser(name, regex, (value) => true));
        }

        public static IParser<CharToken, string> RegexExt(string pattern, Func<string, bool> predicate)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var regex = OptimizeRegex(new Regex(pattern));

            return ParserFactory.CreateEventParser(new RegexParser("Pattern: " + pattern, regex, predicate));
        }

        private static Regex OptimizeRegex(Regex regex)
        {
            return new Regex(string.Format(@"\G{0}", regex), regex.Options);
        }
    }
}
