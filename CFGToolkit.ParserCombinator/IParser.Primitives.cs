using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator
{
    public partial class Parser
    {
        public static IParser<CharToken, string> LineEnd =
            (from r in Char('\r').Optional()
             from n in Char('\n')
             select !r.IsEmpty ? r.Get().ToString() + n : n.ToString())
            .Named("LineEnd");

        public static IParser<CharToken, string> LineTerminator =
            Return<CharToken, string>("").End()
                .XOr(LineEnd.End())
                .XOr(LineEnd)
                .Named("LineTerminator");
    }
}
