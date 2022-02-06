using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator;

namespace CFGToolkit.ParserCombinator.Input
{
    public class InputStream<TToken> : IInputStream<TToken> where TToken : IToken
    {
        private readonly List<TToken> _source;
        
        public InputStream(List<TToken> source, int position, Dictionary<string, object> attributes)
        {
            _source = source;
            Position = position;
            Attributes = attributes;
        }
        public Dictionary<string, object> Attributes { get; set; }

        public List<TToken> Source { get { return _source; } }

        public TToken Current
        {
            get
            {
                return AtEnd ? default : _source[Position];
            }
        }

        public bool AtEnd { get { return Position == _source.Count; } }

        public int Position {
            get;

            private set; 
        }

        public virtual IInputStream<TToken> Advance(int count)
        {
            if (Position + count > _source.Count)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new InputStream<TToken>(_source, Position + count, Attributes);
        }

        public int AdvanceWhile(Func<TToken, bool> predicate, int start)
        {
            int length = 0;
            while ((start + length) < _source.Count)
            {
                if (!predicate(_source[start + length]))
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
            return new InputStream<TToken>(_source, Position, Attributes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_source != null ? _source.GetHashCode() : 0) * 397 ^ Position;
            }
        }

        public override string ToString()
        {
            return "Position: " + Position;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IInputStream<TToken>);
        }

        public bool Equals(IInputStream<TToken> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_source, other.Source) && Position == other.Position;
        }

        public IEnumerator<TToken> GetEnumerator()
        {
            return Source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Source.GetEnumerator();
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

    public static class InputStreamExtensions
    {
        public static string GetText(this IInputStream<CharToken> stream)
        {
            if (!stream.Attributes.ContainsKey("txt"))
            {
                stream.Attributes["txt"] = string.Join(string.Empty, stream.Source.Select(t => t.Value));
            }

            return stream.Attributes["txt"].ToString();
        }
    }
}
