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

            events.Init = (args) =>
            {
                var State = args.GlobalState;

                events.BeforeParse.Add((args) =>
                {
                    if (args.ParserCallStack.Top.TokenSource.Token.IsCancellationRequested)
                    {
                        args.Skip = true;
                        return;
                    }
                });

                var @before = State?.BeforeParseActions;
                if (@before?.TryGetValue(parser.Name, out var list) ?? false)
                {
                    events.BeforeParse.AddRange(list.OrderBy(l => l.Index).Select( l => l.Action));
                }

                var @after = State?.AfterParseActions;
                if (@after?.TryGetValue(parser.Name, out var list2) ?? false)
                {
                    events.AfterParse.AddRange(list2.OrderBy(l => l.Index).Select(l => l.Action));
                }

                events.AfterParse.Add(new Action<AfterArgs<TToken>>((args) =>
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
                }));
                events.Inited = true;
            };

            return events;
        }
    }
}
