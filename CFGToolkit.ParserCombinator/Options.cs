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
    }
}
