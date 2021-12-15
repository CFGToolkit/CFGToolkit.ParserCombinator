using System;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public class ParserCallStack<TToken> : IParserCallStack<TToken> where TToken : IToken
    {
        private Stack<Frame<TToken>> _fullCallStack;

        public ParserCallStack(Frame<TToken> top)
        {
            Top = top;
        }

        public Frame<TToken> Top { get; }

        public Stack<Frame<TToken>> FullStack
        {
            get
            {
                if (_fullCallStack == null)
                {
                    if (Parent != null)
                    {
                        _fullCallStack = new Stack<Frame<TToken>>(Parent.FullStack.Reverse());
                    }
                    else
                    {
                        _fullCallStack = new Stack<Frame<TToken>>(1);
                    }
                    _fullCallStack.Push(Top);
                }
                return _fullCallStack;
            }
        }

        public string FullCallStackText
        {
            get
            {
                return string.Join(Environment.NewLine + "^" + Environment.NewLine, FullStack.Select(o => o.Parser.Name));
            }
        }

        public IParserCallStack<TToken> Parent { get; set; }

        public bool HasParser(string parserName, int depth)
        {
            IParserCallStack<TToken> tmp = this;
            var currentDepth = 0;

            while (tmp != null && currentDepth < depth)
            {
                if (tmp.Top.Parser.Name == parserName)
                {
                    return true;
                }

                tmp = tmp.Parent;
                currentDepth += 1;
            }

            return false;
        }

        public IParserCallStack<TToken> Call(IParser<TToken> parser, IInputStream<TToken> input)
        {
            var frame = new Frame<TToken>() { Parser = parser, Input = input, Parent = Top };
            return new ParserCallStack<TToken>(frame) { Parent = this };
        }
    }
}
