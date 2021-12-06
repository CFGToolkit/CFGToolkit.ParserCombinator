using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator
{
    public interface IInput<TToken> : IEquatable<IInput<TToken>> where TToken : IToken
    {
        IInput<TToken> Advance(int count = 1);
        
        List<TToken> Source { get; }

        TToken Current { get; }

        bool AtEnd { get; }

        int Position { get; }
    }
}
