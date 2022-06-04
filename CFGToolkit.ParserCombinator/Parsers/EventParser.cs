using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Dictionary<string, string> Tags { get; set; }

        public bool Inited { get; set; }

        public Action<BeforeArgs<TToken>> Init { get; set; }

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

            if (Options.Telemetry)
            {
                Telemetry.IncreaseCall(Name);
                watch = new Stopwatch();
                watch.Start();
            }

            if (!Inited && Init != null)
            {
                var beforeArgs = new BeforeArgs<TToken>()
                {
                    GlobalState = globalState,
                    Input = input,
                    ParserCallStack = parserCallStack,
                };

                Init(beforeArgs);
            }

            if (BeforeParse.Count > 0)
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

                    if (beforeArgs.Skip)
                    {
                        return UnionResultFactory.Failure(this, "Cancelled", input);
                    }
                };

                if (beforeArgs.Skip)
                {
                    return UnionResultFactory.Failure(this, "Cancelled", input);
                }
            }

            var result = Parser.Parse(input, globalState, parserCallStack);

            if (AfterParse.Count > 0)
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

            if (Options.Telemetry)
            {
                watch.Stop();
                Telemetry.IncreaseTime(Name, watch.ElapsedMilliseconds);
            }
            return result;
        }
    }
}
