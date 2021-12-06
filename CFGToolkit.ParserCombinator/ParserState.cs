using System;
using System.Collections.Generic;
using System.Linq;

namespace CFGToolkit.ParserCombinator
{
    public class ParserState<TToken> : IParserState<TToken> where TToken : IToken
    {
        private Stack<Frame<TToken>> _fullCallStack;

        public ParserState(Frame<TToken> frame)
        {
            Frame = frame;
        }

        public string Name { get; set; }

        public Frame<TToken> Frame { get; }

        public Stack<Frame<TToken>> FullCallStack
        {
            get
            {
                if (_fullCallStack == null)
                {
                    if (this.Parent != null)
                    {
                        _fullCallStack = new Stack<Frame<TToken>>(this.Parent.FullCallStack.Reverse());
                    }
                    else
                    {
                        _fullCallStack = new Stack<Frame<TToken>>(1);
                    }
                    _fullCallStack.Push(Frame);
                }
                return _fullCallStack;
            }
        }

        public string FullCallStackText
        {
            get
            {
                return string.Join(Environment.NewLine + "^" + Environment.NewLine, FullCallStack.Select(o => o.Parser.Name));
            }
        }

        public ParserState<TToken> Parent { get; set; }

        public bool HasParser(string parserName, int depth = int.MaxValue)
        {
            var tmp = this;
            int currentDepth = 0;

            while (tmp != null && currentDepth < depth)
            {
                if (tmp.Frame.Parser.Name == parserName)
                {
                    return true;
                }

                tmp = tmp.Parent;
                currentDepth += 1;
            }

            return false;
        }

        public ParserState<TToken> Call(IParser<TToken> parser, IInput<TToken> input)
        {
            var frame = new Frame<TToken>() { Parser = parser, Position = input.Position, Input = input, Parent = Frame };
            return new ParserState<TToken>(frame) { Parent = this };
        }
    }
}
