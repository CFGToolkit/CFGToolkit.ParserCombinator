namespace CFGToolkit.ParserCombinator
{
    partial class Parse
    {
        /// <summary>
        /// \n or \r\n
        /// </summary>
        public static IParser<CharToken, string> LineEnd =
            (from r in Char('\r').Optional()
            from n in Char('\n')
            select !r.IsEmpty ? r.Get().ToString() + n : n.ToString())
            .Named("LineEnd");

        /// <summary>
        /// line ending or end of input
        /// </summary>
        public static IParser<CharToken, string> LineTerminator =
            Return<CharToken, string>("").End()
                .XOr(LineEnd.End())
                .XOr(LineEnd)
                .Named("LineTerminator");
    }
}