using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CFGToolkit.ParserCombinator;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public class ParserCallStack<TToken> : IParserCallStack<TToken> where TToken : IToken
    {
        private List<Frame<TToken>> _fullCallStack;

        public ParserCallStack(Frame<TToken> top)
        {
            Top = top;
        }

        public Frame<TToken> Top { get; }

        public List<Frame<TToken>> FullStack
        {
            get
            {
                if (_fullCallStack == null)
                {
                    if (Parent != null)
                    {
                        _fullCallStack = new List<Frame<TToken>>(Parent.FullStack);
                    }
                    else
                    {
                        _fullCallStack = new List<Frame<TToken>>();
                    }

                    _fullCallStack.Add(Top);
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
        
        public Scope<TToken> CurrentScope { get; set; }

        public bool IsPresent(string parserName, int depth)
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

        public bool IsPresent(string[] parserNames, int depth)
        {
            IParserCallStack<TToken> tmp = this;
            var currentDepth = 0;

            while (tmp != null && currentDepth < depth)
            {
                if (parserNames.Contains(tmp.Top.Parser.Name))
                {
                    return true;
                }

                tmp = tmp.Parent;
                currentDepth += 1;
            }

            return false;
        }

        public IParserCallStack<TToken> Call(IParser<TToken> parser, IInputStream<TToken> input, CancellationTokenSource source = null, bool createLinkedTokenSource = false)
        {
            CancellationTokenSource tokenSource;
            if (source == null)
            {
                if (createLinkedTokenSource)
                {
                    tokenSource = CancellationTokenSource.CreateLinkedTokenSource(Top.TokenSource.Token);
                }
                else
                {
                    tokenSource = Top?.TokenSource;
                }
            }
            else
            {
                tokenSource = source;
            }

            var frame = new Frame<TToken>() { Parser = parser, Input = input, Parent = Top, TokenSource = tokenSource };

            return new ParserCallStack<TToken>(frame) { Parent = this, CurrentScope = CurrentScope };
        }
    }
}
