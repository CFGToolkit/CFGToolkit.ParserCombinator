using System;
using System.Text.RegularExpressions;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers;

namespace CFGToolkit.ParserCombinator
{
    public partial class Parser
    {
        public static IParser<CharToken, string> Regex(string pattern, bool token = false, RegexOptions options = RegexOptions.Compiled)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return RegexMatch("Pattern: " + pattern, new Regex(pattern, options), token);
        }

        public static IParser<CharToken, string> RegexMatch(string pattern, bool token = false, RegexOptions options = RegexOptions.Compiled)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return RegexMatch(pattern, new Regex(pattern, options), token);
        }

        public static IParser<CharToken, string> RegexMatch(string name, Regex regex, bool token = false)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));
            regex = OptimizeRegex(regex);
            return new RegexParser(name, regex, (value) => true, token) { ShouldUpdateGlobalState = Options.RegexLevelReporting };
        }

        public static IParser<CharToken, string> RegexExt(string pattern, Func<string, bool> predicate, bool token = false)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var regex = OptimizeRegex(new Regex(pattern));

            return new RegexParser("Pattern: " + pattern, regex, predicate, token) { ShouldUpdateGlobalState = Options.RegexLevelReporting };
        }

        private static Regex OptimizeRegex(Regex regex)
        {
            return new Regex(string.Format(@"\G{0}", regex), regex.Options);
        }
    }
}
