using System;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator
{
    public class ParserException : Exception
    {
        public ParserException() { }

        public ParserException(string message) : base(message) { }


        public ParserException(string message, IUnionResult<CharToken> result) : base(message)
        {
            Result = result;
        }

        public ParserException(string message, Exception innerException) : base(message, innerException) { }

        public IUnionResult<CharToken> Result { get; }
    }
}
