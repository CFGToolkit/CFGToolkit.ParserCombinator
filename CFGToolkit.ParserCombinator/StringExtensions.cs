using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator
{
    static class StringExtensions
    {
        public static IEnumerable<char> ToEnumerable(this string @this)
        {
            return @this;
        }
    }
}
