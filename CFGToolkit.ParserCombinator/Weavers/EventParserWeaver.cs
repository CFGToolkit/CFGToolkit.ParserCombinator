using System;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers;

namespace CFGToolkit.ParserCombinator.Weavers
{
    public class EventParserWeaver : IParserWeaver
    {
        public IParser<TToken, TResult> Create<TToken, TResult>(IParser<TToken, TResult> parser, bool updateState = true) where TToken : IToken
        {
            var events = new EventParser<TToken, TResult>(parser);

            events.BeforeParse.Add(new Action<BeforeArgs<TToken>>((args) =>
            {
                var actions = args.GlobalState.BeforeParseActions;

                if (actions?.ContainsKey(events) ?? false)
                {
                    foreach (var action in actions[events])
                    {
                        action(args);
                    }
                }
                else
                {
                    events.HasBefore = false;
                }
            }));

            events.AfterParse.Add(new Action<AfterArgs<TToken>>((args) =>
            {
                var actions = args.GlobalState.AfterParseActions;

                if (actions?.ContainsKey(events) ?? false)
                {
                    foreach (var action in actions[events])
                    {
                        if (args.Valid)
                        {
                            action(args);
                        }
                    }
                }
                else
                {
                    if (!updateState)
                    {
                        events.HasAfter = false;
                    }
                    else
                    {
                        if (args.ParserResult.WasSuccessful)
                        {
                            var consumed = args.ParserResult.Values.Max(v => v.ConsumedTokens);
                            var consumedPosition = args.Input.Position + (consumed > 0 ? consumed - 1 : 0);
                            if (consumedPosition > args.GlobalState.LastConsumedPosition)
                            {
                                args.GlobalState.LastConsumedPosition = consumedPosition;

                                if (Options.FullErrorReporting)
                                {
                                    args.GlobalState.LastConsumedCallStack = args.ParserCallStack.FullStack;
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
            }));

            return events;
        }
    }
}
