using System;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class EventParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        public EventParser(IParser<TToken, TResult> parser)
        {
            Parser = parser;
        }

        public List<Action<BeforeArgs<TToken>>> BeforeParse { get; } = new List<Action<BeforeArgs<TToken>>>();

        public List<Action<AfterArgs<TToken>>> AfterParse { get; } = new List<Action<AfterArgs<TToken>>>();

        public string Name
        {
            get
            {
                return Parser.Name;
            }
            set
            {
                Parser.Name = value;
            }
        }

        public IParser<TToken, TResult> Parser { get; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> state, IParserCallStack<TToken> parserCallStack)
        {
            if (BeforeParse.Any())
            {
                var beforeArgs = new BeforeArgs<TToken>()
                {
                    GlobalState = state,
                    Input = input,
                    ParserCallStack = parserCallStack,
                };

                foreach (var action in BeforeParse)
                {
                    action(beforeArgs);
                };
            }

            var result = Parser.Parse(input, state, parserCallStack);

            if (AfterParse.Any())
            {
                var afterArgs = new AfterArgs<TToken>()
                {
                    ParserResult = result,
                    GlobalState = state,
                    Input = input,
                    ParserCallStack = parserCallStack,
                };

                foreach (var action in AfterParse)
                {
                    action(afterArgs);
                };
            }

            return result;
        }
    }
}
