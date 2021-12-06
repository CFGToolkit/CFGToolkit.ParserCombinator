using System;

namespace CFGToolkit.ParserCombinator
{
    public interface IOption<out T>
    {
        bool IsEmpty { get; }

        T GetOrDefault();

        T Get();
    }

    internal abstract class AbstractOption<T> : IOption<T>
    {
        public abstract bool IsEmpty { get; }

        public bool IsDefined
        {
            get { return !IsEmpty; }
        }

        public T GetOrDefault()
        {
            return IsEmpty ? default(T) : Get();
        }

        public abstract T Get();
    }

    internal sealed class Some<T> : AbstractOption<T>
    {
        private readonly T _value;

        public Some(T value)
        {
            _value = value;
        }

        public override bool IsEmpty
        {
            get { return false; }
        }

        public override T Get()
        {
            return _value;
        }
    }

    internal sealed class None<T> : AbstractOption<T>
    {
        public override bool IsEmpty
        {
            get { return true; }
        }

        public override T Get()
        {
            throw new InvalidOperationException("Cannot get value from None.");
        }
    }
}
