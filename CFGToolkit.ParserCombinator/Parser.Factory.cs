using System;
using System.Linq;
using CFGToolkit.ParserCombinator.Parsers;

namespace CFGToolkit.ParserCombinator
{
    public class ParserFactory
    {
        public static IParser<TToken, TResult> CreateEventParser<TToken, TResult>(IParser<TToken, TResult> parser) where TToken : IToken
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

                if (args.ParserResult.WasSuccessful)
                {
                    var consumed = args.ParserResult.Values.Max(v => v.ConsumedTokens);
                    var consumedPosition = args.Input.Position + (consumed > 0 ? consumed - 1 : 0);
                    if (consumedPosition > args.GlobalState.LastConsumedPosition)
                    {
                        args.GlobalState.LastConsumedPosition = consumedPosition;
                        args.GlobalState.LastConsumedCallStack = args.ParserState.FullCallStack;
                    }
                }
                else
                {
                    if (args.Input.Position > args.GlobalState.LastFailedPosition)
                    {
                        args.GlobalState.LastFailedPosition = args.Input.Position;
                        args.GlobalState.LastFailedParser = args.ParserState.Frame.Parser;
                    }
                }
                args.ParserState.Frame.Result = args.ParserResult;
            }));

            return events;
        }
    }
}
