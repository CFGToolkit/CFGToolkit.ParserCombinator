using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Input
{
    public interface IInputStream<TToken> : IEquatable<IInputStream<TToken>> where TToken : IToken
    {
        IInputStream<TToken> Advance(int count = 1);

        List<TToken> Source { get; }

        TToken Current { get; }

        bool AtEnd { get; }

        int Position { get; }
    }
}
