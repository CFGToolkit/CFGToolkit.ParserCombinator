using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator
{
    public interface IInputStream<TToken> : IEquatable<IInputStream<TToken>> where TToken : IToken
    {
        IInputStream<TToken> Advance(int count = 1);

        Dictionary<string, object> Attributes { get; set; }

        List<TToken> Source { get; }

        TToken Current { get; }

        bool AtEnd { get; }

        int Position { get; }

        IInputStream<TToken> AdvanceWhile(Func<TToken, bool> predicate, bool clone);

        string ToString();
    }
}
