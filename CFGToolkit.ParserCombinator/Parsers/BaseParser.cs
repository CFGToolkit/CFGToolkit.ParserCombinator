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
                        return UnionResultFactory.Failure(this, "Cancelled", 0, input.Position);
                    }
                }
            }

            var result = ParseInternal(input, globalState, parserCallStack);

            if (AfterParse?.Count > 0)
            {
                var afterArgs = new AfterParseArgs<TToken>()
                {
                    ParserResult = result,
                    GlobalState = globalState,
                    Input = input,
                    ParserCallStack = parserCallStack,
                };
                foreach (var action in AfterParse)
                {
                    action(afterArgs);
                }
            }

            if (ShouldUpdateGlobalState)
            {
                UpdateGlobalState(result, globalState, input, parserCallStack);
            }


            if (Options.Telemetry)
            {
                if (watch != null)
                {
                    watch.Stop();
                    Telemetry.IncreaseTime(Name, watch.ElapsedMilliseconds);
                }
            }
            return result;
        }

        private void UpdateGlobalState(IUnionResult<TToken> parserResult, IGlobalState<TToken> globalState, IInputStream<TToken> input, IParserCallStack<TToken> parserCallStack)
        {
            var consumed = parserResult.MaxConsumed;
            var consumedPosition = input.Position + (consumed > 0 ? consumed - 1 : 0);
            if (parserResult.IsSuccessful)
            {
                if (consumedPosition > globalState.LastConsumedPosition)
                {
                    globalState.LastConsumedPosition = consumedPosition;
                    if (Options.FullErrorReporting)
                    {
                        globalState.LastConsumedCallStack = parserCallStack.FullStack;
                    }
                }
            }
            else
            {
                if (consumedPosition == globalState.LastFailedPosition)
                {
                    if (Options.FullErrorReporting)
                    {
                        globalState.LastFailedCallStacks.Add(parserCallStack.FullStack);
                    }
                }
                else if (consumedPosition > globalState.LastFailedPosition)
                {
                    globalState.LastFailedPosition = consumedPosition;

                    if (Options.FullErrorReporting)
                    {
                        globalState.LastFailedCallStacks.Clear();
                        globalState.LastFailedCallStacks.Add(parserCallStack.FullStack);
                    }
                }
            }

            if (globalState.UpdateHandler != null)
            {
                globalState.UpdateHandler(parserResult.IsSuccessful);
            }
            parserCallStack.Top.Result = parserResult;
        }
    }
}
