using System;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator;

namespace CFGToolkit.ParserCombinator.Input
{
    public class InputStream<TToken> : IInputStream<TToken> where TToken : IToken
    {
        private readonly List<TToken> _source;
        private readonly int _position;

        public InputStream(List<TToken> source, int position)
        {
            _source = source;
            _position = position;
        }

        public virtual IInputStream<TToken> Advance(int count)
        {
            if (_position + count > _source.Count)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new InputStream<TToken>(_source, _position + count);
        }

        public virtual IInputStream<TToken> Clone()
        {
            return new InputStream<TToken>(_source, _position);
        }

        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        public List<TToken> Source { get { return _source; } }

        public TToken Current
        {
            get
            {
                return AtEnd ? default : _source[_position];
            }
        }

        public bool AtEnd { get { return _position == _source.Count; } }

        public int Position { get { return _position; } }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_source != null ? _source.GetHashCode() : 0) * 397 ^ _position;
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
            return Equals(_source, other.Source) && _position == other.Position;
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
