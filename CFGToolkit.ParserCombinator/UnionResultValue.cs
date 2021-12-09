using System;
using System.Linq;

namespace CFGToolkit.ParserCombinator
{

    public class UnionResultValue<TToken> : IUnionResultValue<TToken> where TToken : IToken
    {

        public UnionResultValue(Type valueType)
        {
            ValueType = valueType;
        }

        public bool EmptyMatch { get { return this.ConsumedTokens == 0; } }

        public int ConsumedTokens { get; set; }

        public int Position { get; set; }

        public object Value { get; set; }

        public IInput<TToken> Reminder { get; set; }

        public Type ValueType { get; set; }

        public T GetValue<T>()
        {
            if (Value == null) return default(T);

            return (T)Value;
        }
    }
}
