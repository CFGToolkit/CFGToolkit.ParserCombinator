using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;

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

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            Stopwatch watch = null;

            if (Telemetry.Enabled)
            {
                Telemetry.IncreaseCall(Name);
                watch.Start();
            }

            if (BeforeParse.Any())
            {
                var beforeArgs = new BeforeArgs<TToken>()
                {
                    GlobalState = globalState,
                    Input = input,
                    ParserCallStack = parserCallStack,
                };

                foreach (var action in BeforeParse)
                {
                    action(beforeArgs);
                };
            }

            var result = Parser.Parse(input, globalState, parserCallStack);

            if (AfterParse.Any())
            {
                var afterArgs = new AfterArgs<TToken>()
                {
                    ParserResult = result,
                    GlobalState = globalState,
                    Input = input,
                    ParserCallStack = parserCallStack,
                };

                foreach (var action in AfterParse)
                {
                    action(afterArgs);
                };
            }

            if (Telemetry.Enabled)
            {
                watch.Stop();
                Telemetry.IncreaseTime(Name, watch.ElapsedMilliseconds);
            }
            return result;
        }
    }
}
