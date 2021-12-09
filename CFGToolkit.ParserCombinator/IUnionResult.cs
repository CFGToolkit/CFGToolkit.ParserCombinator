using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator
{
    public interface IUnionResult<TToken> where TToken : IToken
    {
        public bool WasSuccessful { get; }

        IGlobalState<TToken> State { get; set; }

        List<IUnionResultValue<TToken>> Values { get; set; }

        IParser<TToken> Parser { get; set; }

        IInput<TToken> Input { get; set; }
    }
}
