using System;
using System.Collections.Generic;
using System.Linq;

namespace CFGToolkit.ParserCombinator
{

    public class UnionResultValue<TToken> : IUnionResultValue<TToken> where TToken : IToken
    {
        private string _txt;

        public UnionResultValue(Type valueType)
        {
            ValueType = valueType;
        }

        public bool EmptyMatch { get { return this.ConsumedTokens == 0; } }

        public int ConsumedTokens { get; set; }

        public int Position { get; set; }

        public object Value { get; set; }

        public IInput<TToken> Reminder { get; set; }

        public string Text
        {
            get
            {
                if (EmptyMatch)
                {
                    return string.Empty;
                }

                if (_txt == null)
                {
                    _txt = string.Join(string.Empty, Reminder.Source.Skip(Position).Take(ConsumedTokens).Select(token => token.ToString()));
                }
                return _txt;
            }
        }

        public bool WasSuccessful { get; set; } = true;

        public Type ValueType { get; set; }

        public string ErrorMessage { get; set; }

        public T GetValue<T>()
        {
            if (Value == null) return default(T);

            return (T)Value;
        }
    }
}