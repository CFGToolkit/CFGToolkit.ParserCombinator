using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public abstract class BaseParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        public BaseParser()
        {
        }

        public Dictionary<string, string> Tags { get; set; }

        public List<Action<BeforeParseArgs<TToken>>> BeforeParse { get; private set; } 
        
        public List<Action<AfterParseArgs<TToken>>> AfterParse { get; private set; }

        public bool ShouldUpdateGlobalState { get; set; } = true;

        public string Name { get; set; }

        public void EnableEvents()
        {
            if (BeforeParse == null)
            {
                BeforeParse = new List<Action<BeforeParseArgs<TToken>>>();
            }

            if (AfterParse == null)
            {
                AfterParse = new List<Action<AfterParseArgs<TToken>>>();
            }
        }

        protected abstract IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack);

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            Stopwatch watch = null;

            if (Options.Telemetry)
            {
                Telemetry.IncreaseCall(Name);
                watch = new Stopwatch();
                watch.Start();
            }

            if (BeforeParse?.Count > 0)
            {
                var beforeArgs = new BeforeParseArgs<TToken>()
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

            var result = ParseInternal(input, globalState, parserCallStack);

            if (AfterParse?.Count > 0 || ShouldUpdateGlobalState)
            {
                var afterArgs = new AfterParseArgs<TToken>()
                {
                    ParserResult = result,
                    GlobalState = globalState,
                    Input = input,
                    ParserCallStack = parserCallStack,
                };
                if (AfterParse != null)
                {
                    foreach (var action in AfterParse)
                    {
                        action(afterArgs);
                    };
                }

                if (ShouldUpdateGlobalState)
                {
                    UpdateGlobalState(afterArgs);
                }
            }


            if (Options.Telemetry)
            {
                watch.Stop();
                Telemetry.IncreaseTime(Name, watch.ElapsedMilliseconds);
            }
            return result;
        }

        private void UpdateGlobalState(AfterParseArgs<TToken> args)
        {
            if (args.ParserResult.WasSuccessful)
            {
                var consumed = args.ParserResult.Values.Max(v => v.ConsumedTokens);
                var consumedPosition = args.Input.Position + (consumed > 0 ? consumed - 1 : 0);
                if (consumedPosition > args.GlobalState.LastConsumedPosition)
                {
                    args.GlobalState.LastConsumedPosition = consumedPosition;

                    lock (Options.SyncLock)
                    {
                        if (Options.FullErrorReporting)
                        {
                            args.GlobalState.LastConsumedCallStack = args.ParserCallStack.FullStack;
                        }
                    }

                    if (args.GlobalState.UpdateHandler != null)
                    {
                        args.GlobalState.UpdateHandler(true);
                    }
                }
            }
            else
            {
                if (args.Input.Position > args.GlobalState.LastFailedPosition)
                {
                    args.GlobalState.LastFailedPosition = args.Input.Position;
                    args.GlobalState.LastFailedParser = args.ParserCallStack.Top.Parser;

                    if (args.GlobalState.UpdateHandler != null)
                    {
                        args.GlobalState.UpdateHandler(false);
                    }
                }
            }
            args.ParserCallStack.Top.Result = args.ParserResult;
        }
    }
}
