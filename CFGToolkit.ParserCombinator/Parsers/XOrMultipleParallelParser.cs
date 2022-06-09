using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class XOrMultipleParallelParser<TToken, TResult> : BaseParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult>[] _parsers;

        public XOrMultipleParallelParser(string name, IParser<TToken, TResult>[] parsers)
        {
            Name = name;
            _parsers = parsers;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var results = new IUnionResult<TToken>[_parsers.Length];
            int foundIndex = -1;
            bool success = false;
            try
            {
                int i = 0;
                var tasks = new List<Task>();

                using CancellationTokenSource cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(parserCallStack.Top.TokenSource.Token);

                foreach (var parser in _parsers)
                {
                    var j = i;
                    var task = new Task((index) =>
                    {
                        var intIndex = (int)index;
                        var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input, cancellationSource));
                        results[intIndex] = result;

                        if (result.IsSuccessful)
                        {
                            bool prev = true;
                            for (var k = 0; k < intIndex; k++)
                            {
                                if (results[k] == null || results[k].IsSuccessful)
                                {
                                    prev = false;
                                    break;
                                }
                            }
                            success = true;
                            if (prev)
                            {
                                foundIndex = intIndex;
                                if (!cancellationSource.IsCancellationRequested)
                                {
                                    cancellationSource.Cancel();
                                }
                            }
                        }
                    }, j, cancellationSource.Token);

                    if (!task.IsCanceled)
                    {
                        task.Start(Options.TaskScheduler);
                    }
                    tasks.Add(task);
                    i++;
                }

                Task.WaitAll(tasks.ToArray(), cancellationSource.Token);
                var found = success ? foundIndex != -1 ? results[foundIndex] : results.FirstOrDefault(r => r != null && r.IsSuccessful) : null;
                if (found != null)
                {
                    return UnionResultFactory.Success(this, (IUnionResult<TToken>)found);
                }
                return UnionResultFactory.Failure(this, "Parser failed", results.Max(result => result.MaxConsumed), input.Position);
            }
            catch (OperationCanceledException)
            {
                var found = success ? foundIndex != -1 ? results[foundIndex] : results.FirstOrDefault(r => r != null && r.IsSuccessful) : null;
                if (found != null)
                {
                    return UnionResultFactory.Success(this, (IUnionResult<TToken>)found);
                }
                return UnionResultFactory.Failure(this, "Parser failed (cancelled)", results.Max(result => result.MaxConsumed), input.Position);
            }
        }
    }
}
