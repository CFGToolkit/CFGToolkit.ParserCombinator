using System.Collections.Generic;
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


        public static int StartsWith(this IInputStream<CharToken> stream, (int, string)[] values)
        {
            int postion = stream.Position;
            if (postion == stream.Tokens.Count)
            {
                return -1;
            }

            int n = values.Length;
            var failed = new bool[n];
            int failedCounter = 0;

            int j = 0;
            while (postion < stream.Tokens.Count && failedCounter < n)
            {
                for (var i = 0; i < n; i++)
                {
                    if (failed[i])
                    {
                        continue;
                    }

                    if (j < values[i].Item2.Length && stream.Tokens[postion].Value != values[i].Item2[j])
                    {
                        failedCounter++;
                        failed[i] = true;
                    }
                }
                postion++;
                j++;

            }

            if (failedCounter == n)
            {
                return -1;
            }

            for (var i = 0; i < failed.Length; i++)
            {
                if (!failed[i])
                {
                    return values[i].Item1;
                }
            }

            return -1;
        }
    }
}
