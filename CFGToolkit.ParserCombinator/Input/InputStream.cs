using System;
using System.Collections;
using System.Collections.Generic;

namespace CFGToolkit.ParserCombinator.Input
{
    public class InputStream<TToken> : IInputStream<TToken> where TToken : IToken
    {
        private readonly List<TToken> _tokens;
        
        public InputStream(List<TToken> tokens, int position, Dictionary<string, object> attributes)
        {
            _tokens = tokens;
            Position = position;
            Attributes = attributes;
        }
        public Dictionary<string, object> Attributes { get; set; }

        public List<TToken> Tokens { get { return _tokens; } }

        public TToken Current
        {
            get
            {
                return AtEnd ? default : _tokens[Position];
            }
        }

        public bool AtEnd { get { return Position == _tokens.Count; } }

        public int Position {
            get;

            private set; 
        }

        public virtual IInputStream<TToken> Advance(int count)
        {
            if (Position + count > _tokens.Count)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new InputStream<TToken>(_tokens, Position + count, Attributes);
        }

        public int AdvanceWhile(Func<TToken, bool> predicate, int start)
        {
            int length = 0;
            while ((start + length) < _tokens.Count)
            {
                if (!predicate(_tokens[start + length]))
                {
                    break;
                }
                length++;
            }

            return length;
        }

        public IInputStream<TToken> AdvanceWhile(Func<TToken, bool> predicate, bool clone, out int length)
        {
            var tmp = clone ? (InputStream<TToken>)Clone() : this;
            length = 0;
            while (!tmp.AtEnd)
            {
                if (!predicate(tmp.Current))
                {
                    break;
                }
                length++;
                tmp.Position += 1;
            }

            return tmp;
        }

        public virtual IInputStream<TToken> Clone()
        {
            return new InputStream<TToken>(_tokens, Position, Attributes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_tokens != null ? _tokens.GetHashCode() : 0) * 397 ^ Position;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IInputStream<TToken>);
        }

        public bool Equals(IInputStream<TToken> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_tokens, other.Tokens) && Position == other.Position;
        }

        public IEnumerator<TToken> GetEnumerator()
        {
            return Tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Tokens.GetEnumerator();
        }

        public static bool operator ==(InputStream<TToken> left, InputStream<TToken> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InputStream<TToken> left, InputStream<TToken> right)
        {
            return !Equals(left, right);
        }
    }
}
