using System.Linq;
using CFGToolkit.ParserCombinator;

namespace CFGToolkit.ParserCombinator.Input
{
    public static class InputStreamExtensions
    {
        public static string GetText(this IInputStream<CharToken> stream)
        {
            if (!stream.Attributes.ContainsKey("txt"))
            {
                stream.Attributes["txt"] = string.Join(string.Empty, stream.Tokens.Select(t => t.Value));
            }

            return stream.Attributes["txt"].ToString();
        }

        public static string GetReminder(this IInputStream<CharToken> stream, int? position)
        {
            return string.Join(string.Empty, stream.Tokens.Skip(position ?? stream.Position).Select(t => t.Value));
        }
    }
}
