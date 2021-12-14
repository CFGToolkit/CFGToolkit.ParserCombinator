using System;
using System.Collections.Generic;
using System.Linq;

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

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> state, IParserState<TToken> parserState)
        {
            if (BeforeParse.Any())
            {
                var beforeArgs = new BeforeArgs<TToken>()
                {
                    GlobalState = state,
                    Input = input,
                    ParserState = parserState,
                };

                foreach (var action in BeforeParse)
                {
                    action(beforeArgs);
                };
            }

            var result = Parser.Parse(input, state, parserState);

            if (AfterParse.Any())
            {
                var afterArgs = new AfterArgs<TToken>()
                {
                    ParserResult = result,
                    GlobalState = state,
                    Input = input,
                    ParserState = parserState,
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
