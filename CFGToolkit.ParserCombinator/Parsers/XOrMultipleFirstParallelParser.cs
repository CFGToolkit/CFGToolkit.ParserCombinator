﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class XOrMultipleFirstParallelParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly IParser<TToken, TResult>[] _parsers;

        public XOrMultipleFirstParallelParser(string name, IParser<TToken, TResult>[] parsers)
        {
            Name = name;
            _parsers = parsers;
        }

        public string Name { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var results = new IUnionResult<TToken>[_parsers.Length];
            int foundIndex = -1;
            bool success = false;  

            ManualResetEvent eventSuccess = new ManualResetEvent(false);
            CountdownEvent all = new CountdownEvent(_parsers.Length);

            try
            {
                int i = 0;
                var tasks = new List<Task>();

                CancellationTokenSource cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(parserCallStack.Top.TokenSource.Token);

                foreach (var parser in _parsers)
                {
                    var j = i;
                    var task = new Task((index) =>
                    {                       
                        var intIndex = (int)index;
                        var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input, cancellationSource));

                        results[intIndex] = result;

                        if (result.WasSuccessful)
                        {
                            success = true;
                            foundIndex = intIndex;
                            eventSuccess.Set();

                            cancellationSource.Cancel();
                        }

                        all.Signal();

                    }, j, cancellationSource.Token);

                    if (!task.IsCanceled)
                    {
                        task.Start();
                    }
                    tasks.Add(task);
                    i++;
                }

                var x = WaitHandle.WaitAny(new WaitHandle[] { cancellationSource.Token.WaitHandle, eventSuccess, all.WaitHandle });

                var found = success ? foundIndex != -1 ? results[foundIndex] : results.FirstOrDefault(r => r != null && r.WasSuccessful) : null;
                if (found != null)
                {
                    return UnionResultFactory.Success(this, found);
                }
                return UnionResultFactory.Failure(this, "Parser failed", input);
            }
            catch (OperationCanceledException ex)
            {
                var found = success ? foundIndex != -1 ? results[foundIndex] : results.FirstOrDefault(r => r != null && r.WasSuccessful) : null;
                if (found != null)
                {
                    return UnionResultFactory.Success(this, found);
                }
                return UnionResultFactory.Failure(this, "Parser failed (cancelled)", input);
            }
        }
    }
}
