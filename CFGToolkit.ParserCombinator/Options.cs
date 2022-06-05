using System.Threading.Tasks;

namespace CFGToolkit.ParserCombinator
{
    public class Options
    {
        public static bool FullErrorReporting { get; set; } = false;

        public static bool CharLevelReporting { get; set; } = true;

        public static bool StringLevelReporting { get; set; } = true;

        public static bool RegexLevelReporting { get; set; } = true;

        public static bool Telemetry { get; set; } = false;

        public static bool Cache { get; set; } = false;

        public static bool MultiThreading { get; set; } = false;

        public static object SyncLock { get; set; } = new object();

        public static TaskScheduler TaskScheduler { get; set; } = TaskScheduler.Default;
    }
}
