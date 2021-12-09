using System;

namespace CFGToolkit.ParserCombinator
{
    public interface IUnionResultValue<TToken> where TToken : IToken
    {
        int Position { get; set; }

        IInput<TToken> Reminder { get; set; }

        bool EmptyMatch { get; }

        bool Equals(object obj);

        int GetHashCode();

        object Value { get; }

        Type ValueType { get; }

        int ConsumedTokens { get; set; }

        T GetValue<T>();

        string Text { get; }
    }
}
