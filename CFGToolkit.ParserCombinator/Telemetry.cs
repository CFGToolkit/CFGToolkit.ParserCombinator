using System;
using System.Collections.Generic;
using System.Text;

namespace CFGToolkit.ParserCombinator
{
    public class Telemetry
    {
        public static Dictionary<string, long> ParserTotalCalls = new Dictionary<string, long>();

        public static Dictionary<string, long> ParserTotalTime = new Dictionary<string, long>();

        public static void IncreaseCall(string parser)
        {
            if (parser == null)
            {
                parser = "-";
            }
            if (ParserTotalCalls.TryGetValue(parser, out var count))
            {
                ParserTotalCalls[parser] = count + 1;
            }
            else
            {
                ParserTotalCalls[parser] = 1;
            }
        }

        public static void IncreaseTime(string parser, long ms)
        {
            if (parser == null)
            {
                parser = "-";
            }

            if (ParserTotalTime.TryGetValue(parser, out var time))
            {
                ParserTotalTime[parser] = time + ms;
            }
            else
            {
                ParserTotalTime[parser] = ms;
            }
        }

        public static string ExportTime()
        {
            return ExportDictionary(ParserTotalTime);
        }

        public static string ExportAvg()
        {
            var avg = new List<KeyValuePair<string, double>>(ParserTotalCalls.Count);
            foreach (var kvp in ParserTotalCalls)
            {
                if (ParserTotalTime.TryGetValue(kvp.Key, out var time))
                {
                    avg.Add(new KeyValuePair<string, double>(kvp.Key, (double)time / kvp.Value));
                }
            }

            avg.Sort((a, b) => b.Value.CompareTo(a.Value));

            var sb = new StringBuilder();
            for (int i = 0; i < avg.Count; i++)
            {
                if (i > 0) sb.Append(Environment.NewLine);
                sb.Append(avg[i].Key).Append(';').Append(avg[i].Value);
            }
            return sb.ToString();
        }

        public static string ExportCalls()
        {
            return ExportDictionary(ParserTotalCalls);
        }

        private static string ExportDictionary(Dictionary<string, long> dict)
        {
            var sorted = new List<KeyValuePair<string, long>>(dict);
            sorted.Sort((a, b) => b.Value.CompareTo(a.Value));

            var sb = new StringBuilder();
            for (int i = 0; i < sorted.Count; i++)
            {
                if (i > 0) sb.Append(Environment.NewLine);
                sb.Append(sorted[i].Key).Append(';').Append(sorted[i].Value);
            }
            return sb.ToString();
        }
    }
}
