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

            events.AfterParse.Add(new Action<AfterParseArgs<TToken>>((args) =>
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
                    var consumedPosition = args.Input.Position + args.ParserResult.Values.Max(v => v.ConsumedTokens) - 1;
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
            }));

            return events;
        }
    }
}
