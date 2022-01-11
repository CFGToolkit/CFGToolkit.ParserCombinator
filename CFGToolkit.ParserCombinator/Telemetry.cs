using System;
using System.Collections.Generic;
using System.Linq;

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
            if (!ParserTotalCalls.ContainsKey(parser))
            {
                ParserTotalCalls[parser] = 0;
            }
            ParserTotalCalls[parser]++;
        }

        public static void IncreaseTime(string parser, long ms)
        {
            if (parser == null)
            {
                parser = "-";
            }

            if (!ParserTotalTime.ContainsKey(parser))
            {
                ParserTotalTime[parser] = 0;
            }
            ParserTotalTime[parser] += ms;
        }

        public static string ExportTime()
        {
            return string.Join(Environment.NewLine, ParserTotalTime.OrderByDescending(v => v.Value).Select(v => v.Key + ";" + v.Value).ToArray());
        }
        public static string ExportAvg()
        {
            var avg = new Dictionary<string, double>();
            foreach (var p in ParserTotalCalls.Keys)
            {
                avg[p] = (double)ParserTotalTime[p] / (double)ParserTotalCalls[p];
            }

            return string.Join(Environment.NewLine, avg.OrderByDescending(v => v.Value).Select(v => v.Key + ";" + v.Value).ToArray());
        }

        public static string ExportCalls()
        {
            return string.Join(Environment.NewLine, ParserTotalCalls.OrderByDescending(v => v.Value).Select(v => v.Key + ";" + v.Value).ToArray());
        }
    }
}
