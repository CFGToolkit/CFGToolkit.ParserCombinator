using System;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator
{
    public interface IUnionResultValue<TToken> where TToken : IToken
    {
        bool WasSuccessful { get; set; }

        int Position { get; set;  }

        IInput<TToken> Reminder { get; set; }

        bool EmptyMatch { get; }

        bool Equals(object obj);

        int GetHashCode();

        object Value { get; }

        Type ValueType { get; }

        int ConsumedTokens { get; set; }

        T GetValue<T>();

        string Text { get; }

        string ErrorMessage { get; set; }
    }
}