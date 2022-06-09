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
    public class OrMultipleParallelParser<TToken, TResult> : BaseParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult>[] _parsers;

        public OrMultipleParallelParser(string name, IParser<TToken, TResult>[] parsers)
        {
            Name = name;
            _parsers = parsers;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var results = new IUnionResult<TToken>[_parsers.Length];

            try
            {

                int i = 0;
                var tasks = new List<Task>();
                CancellationTokenSource cancellationSource = parserCallStack.Top.TokenSource;

                foreach (var parser in _parsers)
                {
                    var j = i;
                    var task = new Task((index) =>
                    {
                        var intIndex = (int)index;
                        var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input, createLinkedTokenSource: false));
                        results[intIndex] = result;
                    }, j, cancellationSource.Token);

                    if (!task.IsCanceled)
                    {
                        task.Start(Options.TaskScheduler);
                    }
                    tasks.Add(task);
                    i++;
                }

                Task.WaitAll(tasks.ToArray(), cancellationSource.Token);

                var values = new List<IUnionResultValue<TToken>>();
                foreach (var result in results)
                {
                    if (result.IsSuccessful)
                    {
                        values.AddRange(result.Values);
                    }
                }

                if (values.Count > 0)
                {
                    return UnionResultFactory.Success(this, values);
                }
                else
                {
                    return UnionResultFactory.Failure(this, "Parser failed", results.Max(result => result?.MaxConsumed ?? 0), input.Position);
                }
            }
            catch (Exception)
            {
                return UnionResultFactory.Failure(this, "Parser cancelled", results.Max(result => result?.MaxConsumed ?? 0), input.Position);
            }
        }
    }
}
