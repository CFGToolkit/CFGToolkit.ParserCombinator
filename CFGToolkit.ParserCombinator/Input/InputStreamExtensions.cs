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

        public static bool StartsWith(this IInputStream<CharToken> stream, string value)
        {
            int postion = stream.Position;
            int strPos = 0;

            if (postion == stream.Tokens.Count)
            {
                return false;
            }

            while (strPos < value.Length && postion < stream.Tokens.Count)
            {
                if (stream.Tokens[postion].Value != value[strPos])
                {
                    return false;
                }
                postion++;
                strPos++;
            }

            return true;
        }
    }
}
