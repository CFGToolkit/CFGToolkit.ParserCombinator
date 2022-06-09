using System;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.Values
{
    public class UnionResultValue<TToken> : IUnionResultValue<TToken> where TToken : IToken
    {
        public UnionResultValue(Type valueType)
        {
            ValueType = valueType;
        }

        public bool EmptyMatch { get { return ConsumedTokens == 0; } }

        public int ConsumedTokens { get; set; }

        public bool IsSuccessful { get; set; }

        public int Position { get; set; }

        public object Value { get; set; }

        public IInputStream<TToken> Reminder { get; set; }

        public Type ValueType { get; set; }

        public T GetValue<T>()
        {
            if (Value == null) return default;

            return (T)Value;
        }
    }
}
