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

    public class InputStream : InputStream<CharToken>
    {
        private string _text = null;
        private string _currentText = null;

        public InputStream(List<CharToken> source, int position = 0, string text = null) : base(source, position)
        {
            _text = text;
        }

        public string SourceText
        {
            get
            {

                if (_text == null)
                {
                    _text = string.Join(string.Empty, Source.Select(t => t.Value));
                }

                return _text;
            }
        }

        public string CurrentText
        {
            get
            {
                if (_currentText == null)
                {
                    _currentText = string.Join(string.Empty, Source.Skip(Position).Select(t => t.Value));
                }

                return _currentText;
            }
        }

        public override IInputStream<CharToken> Advance(int count)
        {
            if (Position + count > Source.Count)
                throw new InvalidOperationException("Too far");

            return new InputStream(Source, Position + count, _text);
        }

        public override IInputStream<CharToken> Clone()
        {
            return new InputStream(Source, Position, _text);
        }
    }
}
